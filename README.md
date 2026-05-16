🚀 AR Stress-Adaptive Shooting System
📌 Overview

This project is a real-time stress-adaptive AR shooting application that integrates physiological sensing, machine learning, and augmented reality.

The system captures ECG signals, processes heart rate variability (HRV) features, predicts stress levels using a trained ML model, and dynamically adapts gameplay in a Unity-based AR environment.

🧠 System Architecture
ECG Sensor (AD8232)
        ↓
Arduino (RR Interval Extraction)
        ↓
Flask API (/sensor_input)
        ↓
Feature Extraction (HR, RMSSD, SDNN)
        ↓
ML Model (Stress Prediction)
        ↓
API Endpoint (/predict_live)
        ↓
Unity AR Application
⚙️ Components
🔹 Hardware (hardware/)
AD8232 ECG Sensor
Arduino for signal acquisition
RR interval detection logic
🔹 Backend (backend/)
Flask-based API for real-time processing

Accepts RR intervals via:

POST /sensor_input

Provides live stress prediction via:

GET /predict_live
🔹 Machine Learning (ml/)
Trained model using HRV features:
Heart Rate Mean
Heart Rate Std
RMSSD
SDNN
Model and scaler saved as .pkl files
🔹 Unity AR Application (unity_ar_app/)
AR shooting environment
Fetches stress data from API
📊 Features
Real-time ECG signal processing
HRV feature extraction
ML-based stress classification
REST API integration
AR-based adaptive gameplay
Live stress visualization
🔌 API Endpoints
1. Send Sensor Data
POST /sensor_input

Request:

{
  "rr_value": 850
}
2. Get Live Prediction
GET /predict_live

Response:

{
  "rr_value": 820,
  "features": {
    "hr_mean": 72.5,
    "hr_std": 3.2,
    "rmssd": 45.1,
    "sdnn": 50.3
  },
  "probability": 0.82,
  "state": "HIGH STRESS"
}
🛠️ How to Run
🔹 Backend
cd backend
pip install -r requirements.txt
python app.py
🔹 Unity
Open unity_ar_app in Unity Hub
Attach StressAPI.cs script

Set API URL:

http://<your-ip>:5000/predict_live
Play the scene
📡 Network Setup
Ensure Unity device and backend server are on the same network
Replace localhost with your system IP
🚀 Future Improvements
Session-based tracking
Multi-user support
Advanced ML models (LSTM, real-time sequence modeling)
Cloud deployment of API
Integration with wearable devices
🎯 Applications
Cognitive load monitoring
Adaptive gaming systems
Pilot/driver stress monitoring
Human-computer interaction research
📌 Tech Stack
Python (Flask, NumPy, Scikit-learn)
Unity (C#)
Arduino (C++)
Machine Learning (HRV-based classification)
👨‍💻 Author

Karthic N A

⭐ Notes

This project demonstrates an end-to-end pipeline combining biosignal processing, machine learning, and immersive AR interaction.
