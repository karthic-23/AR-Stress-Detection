/*
  Improved AD8232 ECG -> RR Detection
  Stable + Real-time Ready
*/

unsigned long lastPeakTime = 0;

bool firstBeat = true;
bool peakDetected = false;

// Adaptive threshold
int threshold = 600;

// Refractory period (ms) → prevents double detection
const int refractoryPeriod = 250;

void setup() {

  Serial.begin(9600);

  pinMode(10, INPUT);
  pinMode(11, INPUT);

  Serial.println("ECG Started...");
}

void loop() {

  // =========================================
  // CHECK ELECTRODES
  // =========================================

  if ((digitalRead(10) == 1) || (digitalRead(11) == 1)) {
    Serial.println("Leads off");
    delay(200);
    return;
  }

  // =========================================
  // READ ECG
  // =========================================

  int ecgValue = analogRead(A0);

  // =========================================
  // SIMPLE ADAPTIVE THRESHOLD
  // =========================================

  threshold = threshold * 0.9 + ecgValue * 0.1;

  // =========================================
  // PEAK DETECTION
  // =========================================

  if (ecgValue > threshold + 30 && !peakDetected) {

    unsigned long currentTime = millis();

    // Refractory check
    if (currentTime - lastPeakTime > refractoryPeriod) {

      if (!firstBeat) {

        unsigned long rrInterval =
          currentTime - lastPeakTime;

        // Valid RR range (40 bpm – 200 bpm)
        if (rrInterval > 300 && rrInterval < 1500) {

          Serial.print("RR:");
          Serial.println(rrInterval);
        }
      }

      firstBeat = false;
      lastPeakTime = currentTime;
      peakDetected = true;
    }
  }

  // =========================================
  // RESET PEAK FLAG
  // =========================================

  if (ecgValue < threshold) {
    peakDetected = false;
  }

  delay(5);
}