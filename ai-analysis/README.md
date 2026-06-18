---
title: Breast Cancer Segmentation API
emoji: 🔬
colorFrom: pink
colorTo: red
sdk: docker
app_port: 7860
pinned: false
---

# 🔬 Breast Cancer Segmentation API

A Flask REST API that accepts an ultrasound image and returns a **segmentation mask** using a trained Keras model.

## Endpoints

### `GET /`
Returns available endpoints.

### `POST /predict`
Returns the predicted mask as a **PNG image**.

```bash
curl -X POST https://<your-space>.hf.space/predict \
  -F "image=@your_image.png" \
  --output mask.png
```

### `POST /predict/json`
Returns JSON with label, confidence, and base64-encoded mask.

```bash
curl -X POST https://<your-space>.hf.space/predict/json \
  -F "image=@your_image.png"
```

**Response:**
```json
{
  "label": "Malignant",
  "confidence": 0.7231,
  "mask_coverage": 0.0412,
  "mask_base64": "<base64 PNG string>"
}
```

## Setup on Hugging Face Spaces

1. Create a new Space → choose **Docker** as SDK
2. Upload all files:
   - `app.py`
   - `requirements.txt`
   - `Dockerfile`
   - `model_breast_cancer.h5` ← your trained model
3. The Space will build and deploy automatically
