from flask import Flask, render_template, request, jsonify, Response, stream_with_context
from groq import Groq
import json
import os

GROQ_API_KEY = os.environ.get("GROQ_API_KEY", "")

app = Flask(__name__)
client = Groq(api_key=GROQ_API_KEY)

with open('breastCancer.json', encoding='utf-8') as f:
    raw_data = json.load(f)

# بناء lookup dictionary للبحث السريع
KB_DICT = {}
for item in raw_data['intents']:
    tag = item['tags'].strip()
    resp = ''.join(item['responses'])[:300].replace('\n', ' ')
    patterns = [p.lower() for p in item['patterns']]
    KB_DICT[tag] = {'patterns': patterns, 'response': resp}

def find_relevant_kb(user_message, top_k=3):
    """ابحث عن أقرب إجابات من الـ KB بناءً على كلمات المستخدم"""
    msg = user_message.lower()
    scores = []
    for tag, data in KB_DICT.items():
        score = 0
        for pattern in data['patterns']:
            words = pattern.split()
            matches = sum(1 for w in words if w in msg)
            score = max(score, matches)
        if score > 0:
            scores.append((score, tag, data['response']))
    scores.sort(reverse=True)
    results = scores[:top_k]
    if not results:
        return ""
    return '\n'.join([f"[{tag}]: {resp}" for _, tag, resp in results])

SYSTEM_PROMPT_BASE = """You are a compassionate medical assistant specializing in breast cancer.
Respond in the SAME language the user uses (Arabic or English).

Rules:
- Be warm and empathetic
- Use bullet points for lists
- Always recommend consulting a doctor
- Never diagnose
- Arabic replies: use clear Modern Standard Arabic
- Keep answers concise and clear
"""

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/chat', methods=['POST'])
def chat():
    data = request.get_json()
    messages = data.get('messages', [])
    if not messages:
        return jsonify({'error': 'No messages provided'}), 400

    # آخر سؤال للمستخدم
    last_user_msg = ""
    for m in reversed(messages):
        if m['role'] == 'user':
            last_user_msg = m['content']
            break

    # جيب فقط الـ KB المتعلقة بالسؤال
    relevant_kb = find_relevant_kb(last_user_msg)
    kb_section = f"\nRELEVANT INFO:\n{relevant_kb}\n" if relevant_kb else ""

    system_content = SYSTEM_PROMPT_BASE + kb_section
    recent_messages = messages[-4:]  # آخر 4 رسائل بس
    full_messages = [{"role": "system", "content": system_content}] + recent_messages

    def generate():
        try:
            stream = client.chat.completions.create(
                model="llama-3.1-8b-instant",
                messages=full_messages,
                max_tokens=600,
                temperature=0.3,
                stream=True,
            )
            for chunk in stream:
                text = chunk.choices[0].delta.content or ""
                if text:
                    yield f"data: {json.dumps({'text': text})}\n\n"
            yield "data: [DONE]\n\n"
        except Exception as e:
            yield f"data: {json.dumps({'text': f'Error: {str(e)}'})}\n\n"
            yield "data: [DONE]\n\n"

    return Response(
        stream_with_context(generate()),
        mimetype='text/event-stream',
        headers={'Cache-Control': 'no-cache', 'X-Accel-Buffering': 'no'}
    )

@app.route('/suggest', methods=['POST'])
def suggest():
    data = request.get_json()
    last_response = data.get('last_response', '')
    language = data.get('language', 'en')

    lang_word = 'Arabic' if language == 'ar' else 'English'
    prompt = (
        f"Topic: {last_response[:150]}\n"
        f"Give 3 short follow-up questions in {lang_word}.\n"
        'Return ONLY a JSON array: ["Q1?", "Q2?", "Q3?"]'
    )

    try:
        result = client.chat.completions.create(
            model="llama-3.1-8b-instant",
            messages=[{"role": "user", "content": prompt}],
            max_tokens=120
        )
        text = result.choices[0].message.content.strip().strip('```json').strip('```').strip()
        suggestions = json.loads(text)
        return jsonify({'suggestions': suggestions})
    except Exception:
        return jsonify({'suggestions': []})

if __name__ == '__main__':
    port = int(os.environ.get("PORT", 7860))
    print(f"\n{'='*50}")
    print(f"  🚀 Server running on port {port}")
    print(f"{'='*50}\n")
    app.run(host="0.0.0.0", port=port, debug=False)