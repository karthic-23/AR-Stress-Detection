import numpy as np
import joblib
from flask import Flask, jsonify, request

# =========================================================
# CONFIGURATION
# =========================================================

WINDOW_SIZE = 5

app = Flask(__name__)

prob_history = []
rr_buffer = []

# =========================================================
# LOAD MODEL
# =========================================================

try:
    model = joblib.load("stress_model.pkl")
    scaler = joblib.load("scaler.pkl")
    print("✅ Model loaded successfully")

except Exception as e:
    print("❌ Model loading failed:", e)
    exit()

# =========================================================
# GLOBAL DATA
# =========================================================

current_stress_data = {
    "rr_value": 0,
    "features": {
        "hr_mean": 0,
        "hr_std": 0,
        "rmssd": 0,
        "sdnn": 0
    },
    "probability": 0.0,
    "state": "WAITING FOR SENSOR DATA"
}

# =========================================================
# PROCESS RR WINDOW
# =========================================================

def process_rr_window(rr_intervals):

    global current_stress_data

    rr_array = np.array(rr_intervals)

    # =====================================================
    # FEATURES
    # =====================================================

    hr_array = 60000.0 / rr_array

    hr_mean = float(np.mean(hr_array))
    hr_std = float(np.std(hr_array))
    sdnn = float(np.std(rr_array))

    rr_diff = np.diff(rr_array)
    rmssd = float(np.sqrt(np.mean(rr_diff ** 2)))

    features = [hr_mean, hr_std, rmssd, sdnn]

    # =====================================================
    # MODEL PREDICTION
    # =====================================================

    X = np.array(features).reshape(1, -1)
    X_scaled = scaler.transform(X)

    proba = float(model.predict_proba(X_scaled)[0][1])

    # =====================================================
    # SMOOTHING
    # =====================================================

    prob_history.append(proba)

    if len(prob_history) > 3:
        prob_history.pop(0)

    smooth_proba = np.mean(prob_history)

    # =====================================================
    # LABEL
    # =====================================================

    if smooth_proba > 0.7:
        state = "HIGH STRESS"
    else:
        state = "MODERATE"

    # =====================================================
    # DEBUG
    # =====================================================

    print("\n===================================")
    print("RR WINDOW:", rr_intervals)
    print(f"HR Mean : {hr_mean:.2f}")
    print(f"HR Std  : {hr_std:.2f}")
    print(f"RMSSD   : {rmssd:.2f}")
    print(f"SDNN    : {sdnn:.2f}")
    print(f"Probability : {smooth_proba:.4f}")
    print(f"STATE : {state}")
    print("===================================\n")

    # =====================================================
    # UPDATE OUTPUT
    # =====================================================

    current_stress_data = {
        "rr_value": rr_intervals[-1],
        "features": {
            "hr_mean": round(hr_mean, 2),
            "hr_std": round(hr_std, 2),
            "rmssd": round(rmssd, 2),
            "sdnn": round(sdnn, 2)
        },
        "probability": round(smooth_proba, 4),
        "state": state
    }

# =========================================================
# SENSOR INPUT API
# =========================================================

@app.route('/sensor_input', methods=['POST'])
def sensor_input():

    global rr_buffer

    data = request.get_json()

    if not data or "rr_value" not in data:
        return jsonify({"error": "rr_value missing"}), 400

    rr = data["rr_value"]

    # Basic validation
    if rr <= 0:
        return jsonify({"error": "Invalid RR value"}), 400

    rr_buffer.append(rr)

    if len(rr_buffer) > WINDOW_SIZE:
        rr_buffer.pop(0)

    if len(rr_buffer) == WINDOW_SIZE:
        process_rr_window(rr_buffer)

    return jsonify({
        "message": "RR received",
        "buffer_size": len(rr_buffer)
    })

# =========================================================
# GET RESULT
# =========================================================

@app.route('/predict_live', methods=['GET'])
def predict_live():
    return jsonify(current_stress_data)

@app.route('/')
def home():
    return """
    <h1>Stress Detection API (Sensor Mode)</h1>
    <p>POST RR data to /sensor_input</p>
    <p>GET results from /predict_live</p>
    """

# =========================================================
# MAIN
# =========================================================

if __name__ == '__main__':

    print("✅ Running in SENSOR MODE (No synthetic data)")

    app.run(
        host='0.0.0.0',
        port=5000
    )