import os
import io
import cv2
import numpy as np
from flask import Flask, request, jsonify, send_file
from tensorflow.keras.models import load_model
from PIL import Image
import base64
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

# ─── Load model once at startup ───────────────────────────────────────────────
MODEL_PATH = "model_breast_cancer.h5"

if not os.path.exists(MODEL_PATH):
    raise FileNotFoundError(
        f"Model file '{MODEL_PATH}' not found. "
        "Please upload model_breast_cancer.h5 to the Space."
    )

model = load_model(MODEL_PATH, compile=False)
print("✅ Model loaded successfully")


# ─── Helpers ──────────────────────────────────────────────────────────────────
def preprocess(image_bytes: bytes) -> np.ndarray:
    np_arr = np.frombuffer(image_bytes, np.uint8)
    img = cv2.imdecode(np_arr, cv2.IMREAD_GRAYSCALE)
    if img is None:
        raise ValueError("Could not decode the uploaded image.")
    img = cv2.resize(img, (128, 128), interpolation=cv2.INTER_AREA)
    img = img.astype("float32") / 255.0
    img = np.expand_dims(img, axis=-1)
    img = np.expand_dims(img, axis=0)
    return img


def mask_to_png_bytes(mask: np.ndarray) -> bytes:
    mask_uint8 = (mask.squeeze() * 255).astype(np.uint8)
    pil_img = Image.fromarray(mask_uint8, mode="L")
    buf = io.BytesIO()
    pil_img.save(buf, format="PNG")
    buf.seek(0)
    return buf.read()


# ─── Embedded HTML UI ─────────────────────────────────────────────────────────
HTML_PAGE = """<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8"/>
<meta name="viewport" content="width=device-width, initial-scale=1.0"/>
<title>Breast Cancer Segmentation</title>
<style>
  @import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@300;400;500;600&family=DM+Serif+Display&display=swap');
  *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
  :root {
    --pink: #e91e8c; --rose: #ff4d6d;
    --dark: #0f0f14; --card: #16161e;
    --border: #2a2a38; --text: #e8e8f0; --muted: #6b6b80;
  }
  body {
    font-family: 'DM Sans', sans-serif;
    background: var(--dark); color: var(--text);
    min-height: 100vh; display: flex; flex-direction: column;
    align-items: center; justify-content: center; padding: 2rem 1rem;
  }
  .header { text-align: center; margin-bottom: 2.5rem; }
  .header h1 {
    font-family: 'DM Serif Display', serif;
    font-size: clamp(1.8rem, 4vw, 2.8rem);
    background: linear-gradient(135deg, #fff 30%, var(--pink));
    -webkit-background-clip: text; -webkit-text-fill-color: transparent;
    margin-bottom: .5rem;
  }
  .header p { color: var(--muted); font-size: .95rem; }
  .card {
    width: 100%; max-width: 700px;
    background: var(--card); border: 1px solid var(--border);
    border-radius: 20px; padding: 2rem;
  }
  .drop-zone {
    border: 2px dashed var(--border); border-radius: 14px;
    padding: 2.5rem 1rem; text-align: center; cursor: pointer;
    transition: border-color .25s, background .25s; position: relative;
  }
  .drop-zone:hover, .drop-zone.drag {
    border-color: var(--pink); background: rgba(233,30,140,.05);
  }
  .drop-zone input { position: absolute; inset: 0; opacity: 0; cursor: pointer; }
  .drop-zone .icon { font-size: 2.5rem; margin-bottom: .75rem; }
  .drop-zone p { color: var(--muted); font-size: .9rem; }
  .drop-zone p span { color: var(--pink); font-weight: 600; }
  #preview-wrap { display: none; margin-top: 1.25rem; }
  #preview-wrap img {
    width: 100%; max-height: 220px; object-fit: cover;
    border-radius: 10px; border: 1px solid var(--border);
  }
  .file-info {
    display: flex; align-items: center; justify-content: space-between;
    margin-top: .6rem; font-size: .82rem; color: var(--muted);
  }
  .remove-btn {
    background: none; border: none; cursor: pointer; color: #ff4d6d;
    font-size: .85rem; padding: 2px 8px; border-radius: 6px;
    transition: background .2s;
  }
  .remove-btn:hover { background: rgba(255,77,109,.12); }
  .analyze-btn {
    width: 100%; margin-top: 1.5rem; padding: .95rem;
    background: linear-gradient(135deg, var(--pink), var(--rose));
    border: none; border-radius: 12px; color: #fff;
    font-family: inherit; font-size: 1rem; font-weight: 600;
    cursor: pointer; letter-spacing: .02em;
    transition: opacity .2s, transform .15s;
    display: flex; align-items: center; justify-content: center; gap: .5rem;
  }
  .analyze-btn:hover:not(:disabled) { opacity: .9; transform: translateY(-1px); }
  .analyze-btn:disabled { opacity: .4; cursor: not-allowed; transform: none; }
  .spinner {
    width: 18px; height: 18px;
    border: 2px solid rgba(255,255,255,.3); border-top-color: #fff;
    border-radius: 50%; animation: spin .7s linear infinite; display: none;
  }
  @keyframes spin { to { transform: rotate(360deg); } }
  #results { display: none; margin-top: 1.75rem; }
  .result-header {
    display: flex; align-items: center; justify-content: space-between;
    margin-bottom: 1rem;
  }
  .result-header h3 { font-size: 1rem; font-weight: 600; }
  .badge {
    padding: .35rem .9rem; border-radius: 999px;
    font-size: .82rem; font-weight: 700; letter-spacing: .04em;
  }
  .badge.malignant { background: rgba(255,77,109,.15); color: #ff4d6d; }
  .badge.benign    { background: rgba(52,199,89,.15);  color: #34c759; }
  .images-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
  .img-box { text-align: center; }
  .img-box p {
    font-size: .75rem; text-transform: uppercase; letter-spacing: .08em;
    color: var(--muted); margin-bottom: .5rem;
  }
  .img-box img {
    width: 100%; border-radius: 10px; border: 1px solid var(--border);
    aspect-ratio: 1; object-fit: cover;
  }
  .error-box {
    display: none; margin-top: 1rem; padding: .85rem 1rem; border-radius: 10px;
    background: rgba(255,77,109,.1); border: 1px solid rgba(255,77,109,.3);
    color: #ff4d6d; font-size: .88rem;
  }
  .disclaimer {
    margin-top: 1.5rem; padding: .85rem 1rem;
    background: rgba(255,193,7,.06); border: 1px solid rgba(255,193,7,.2);
    border-radius: 10px; font-size: .8rem; color: #a89060; line-height: 1.5;
  }
  .disclaimer strong { color: #ffc107; }
</style>
</head>
<body>
<div class="header">
  <h1>🔬 Breast Cancer Segmentation</h1>
  <p>Upload an ultrasound image — AI highlights suspicious regions instantly</p>
</div>
<div class="card">
  <div class="drop-zone" id="dropZone">
    <input type="file" id="fileInput" accept="image/*" />
    <div class="icon">🩻</div>
    <p><span>Click to upload</span> or drag &amp; drop</p>
    <p style="margin-top:.3rem">JPG, PNG supported</p>
  </div>
  <div id="preview-wrap">
    <img id="preview-img" src="" alt="preview" />
    <div class="file-info">
      <span id="file-name"></span>
      <button class="remove-btn" onclick="removeFile()">✕ Remove</button>
    </div>
  </div>
  <button class="analyze-btn" id="analyzeBtn" disabled onclick="analyze()">
    <div class="spinner" id="spinner"></div>
    <span id="btn-label">Analyze Image</span>
  </button>
  <div class="error-box" id="errorBox"></div>
  <div id="results">
    <div class="result-header">
      <h3>Analysis Result</h3>
      <div class="badge" id="labelBadge"></div>
    </div>
    <div class="images-grid">
      <div class="img-box">
        <p>Original</p>
        <img id="origImg" src="" alt="original" />
      </div>
      <div class="img-box">
        <p>Segmentation Mask</p>
        <img id="maskImg" src="" alt="mask" />
      </div>
    </div>
  </div>
  <div class="disclaimer">
    <strong>⚠️ Disclaimer:</strong> This tool is for research purposes only and does not constitute a medical diagnosis. Always consult a qualified healthcare professional.
  </div>
</div>
<script>
  let selectedFile = null;
  const dropZone    = document.getElementById('dropZone');
  const fileInput   = document.getElementById('fileInput');
  const analyzeBtn  = document.getElementById('analyzeBtn');
  const spinner     = document.getElementById('spinner');
  const btnLabel    = document.getElementById('btn-label');
  const previewWrap = document.getElementById('preview-wrap');
  const previewImg  = document.getElementById('preview-img');
  const fileName    = document.getElementById('file-name');
  const results     = document.getElementById('results');
  const errorBox    = document.getElementById('errorBox');
  dropZone.addEventListener('dragover', e => { e.preventDefault(); dropZone.classList.add('drag'); });
  dropZone.addEventListener('dragleave', () => dropZone.classList.remove('drag'));
  dropZone.addEventListener('drop', e => {
    e.preventDefault(); dropZone.classList.remove('drag');
    const f = e.dataTransfer.files[0];
    if (f && f.type.startsWith('image/')) setFile(f);
  });
  fileInput.addEventListener('change', () => { if (fileInput.files[0]) setFile(fileInput.files[0]); });
  function setFile(f) {
    selectedFile = f;
    previewImg.src = URL.createObjectURL(f);
    fileName.textContent = f.name + ' (' + (f.size / 1024).toFixed(1) + ' KB)';
    previewWrap.style.display = 'block';
    results.style.display = 'none';
    errorBox.style.display = 'none';
    analyzeBtn.disabled = false;
  }
  function removeFile() {
    selectedFile = null; fileInput.value = '';
    previewWrap.style.display = 'none';
    results.style.display = 'none';
    errorBox.style.display = 'none';
    analyzeBtn.disabled = true;
  }
  async function analyze() {
    if (!selectedFile) return;
    analyzeBtn.disabled = true;
    spinner.style.display = 'block';
    btnLabel.textContent = 'Analyzing...';
    results.style.display = 'none';
    errorBox.style.display = 'none';
    const formData = new FormData();
    formData.append('image', selectedFile);
    try {
      const res = await fetch('/predict/json', { method: 'POST', body: formData });
      if (!res.ok) { const err = await res.json(); throw new Error(err.error || 'Server error'); }
      const data = await res.json();
      showResults(data);
    } catch (e) {
      errorBox.textContent = 'Error: ' + e.message;
      errorBox.style.display = 'block';
    } finally {
      analyzeBtn.disabled = false;
      spinner.style.display = 'none';
      btnLabel.textContent = 'Analyze Image';
    }
  }
  function showResults(data) {
    const isM = data.label === 'Malignant';
    const badge = document.getElementById('labelBadge');
    badge.textContent = data.label;
    badge.className = 'badge ' + (isM ? 'malignant' : 'benign');
    document.getElementById('origImg').src = URL.createObjectURL(selectedFile);
    document.getElementById('maskImg').src = 'data:image/png;base64,' + data.mask_base64;
    results.style.display = 'block';
    results.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
  }
</script>
</body>
</html>
"""


# ─── Routes ───────────────────────────────────────────────────────────────────
@app.route("/", methods=["GET"])
def index():
    return HTML_PAGE


@app.route("/predict", methods=["POST"])
def predict_image():
    if "image" not in request.files:
        return jsonify({"error": "No 'image' field in request"}), 400
    file = request.files["image"]
    if file.filename == "":
        return jsonify({"error": "Empty filename"}), 400
    try:
        image_bytes = file.read()
        tensor = preprocess(image_bytes)
        prediction = model.predict(tensor)
        pred_mask = (prediction[0] > 0.5).astype(np.uint8)
        png_bytes = mask_to_png_bytes(pred_mask)
        return send_file(io.BytesIO(png_bytes), mimetype="image/png",
                         as_attachment=False, download_name="mask.png")
    except ValueError as e:
        return jsonify({"error": str(e)}), 422
    except Exception as e:
        return jsonify({"error": f"Internal error: {str(e)}"}), 500


@app.route("/predict/json", methods=["POST"])
def predict_json():
    if "image" not in request.files:
        return jsonify({"error": "No 'image' field in request"}), 400
    file = request.files["image"]
    if file.filename == "":
        return jsonify({"error": "Empty filename"}), 400
    try:
        image_bytes = file.read()
        tensor = preprocess(image_bytes)
        prediction = model.predict(tensor)
        pred_mask = (prediction[0] > 0.5).astype(np.uint8)
        mask_coverage = float(np.mean(pred_mask))
        label = "Malignant" if mask_coverage > 0.01 else "Benign"
        png_bytes = mask_to_png_bytes(pred_mask)
        mask_b64 = base64.b64encode(png_bytes).decode("utf-8")
        return jsonify({
            "label": label,
            "mask_base64": mask_b64
        })
    except ValueError as e:
        return jsonify({"error": str(e)}), 422
    except Exception as e:
        return jsonify({"error": f"Internal error: {str(e)}"}), 500


# ─── Entry point ──────────────────────────────────────────────────────────────
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=7860, debug=False)