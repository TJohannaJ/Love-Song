import os
import tempfile
import requests
import essentia.standard as es
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel

app = FastAPI()

class AnalysisRequest(BaseModel):
    url: str

@app.post("/analyze")
def analyze_audio(request: AnalysisRequest):
    if not request.url:
        raise HTTPException(status_code=400, detail="Hiányzó URL!")

    with tempfile.NamedTemporaryFile(delete=False, suffix=".mp3") as tmp_file:
        tmp_path = tmp_file.name

    try:
        # 1. MP3 Letöltése
        headers = {"User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"}
        res = requests.get(request.url, headers=headers, timeout=10)
        if res.status_code != 200:
            raise HTTPException(status_code=400, detail="Nem tölthető le az audio")
        
        with open(tmp_path, "wb") as f:
            f.write(res.content)

        # 2. Audio betöltése
        loader = es.MonoLoader(filename=tmp_path, sampleRate=44100)
        audio = loader()

        # 3. Essentia Algoritmusok & Korrigált 0.0 - 1.0 skálázás
        
        # BPM (200 BPM-re normálva)
        bpm, _, _, _, _ = es.RhythmExtractor2013()(audio)
        normalized_bpm = min(max(float(bpm) / 200.0, 0.0), 1.0)

        # Danceability (Essentia tipikusan 0.0 - 2.5 között adja vissza)
        raw_danceability, _ = es.Danceability()(audio)
        danceability = min(max(float(raw_danceability) / 2.2, 0.0), 1.0)

        # Energy (Nyers érték 30s MP3-nál ~200,000 körül van -> 300,000 osztóval normálva)
        raw_energy = es.Energy()(audio)
        energy = min(max(float(raw_energy) / 300000.0, 0.0), 1.0)

        # Acousticness (DynamicComplexity alapján)
        dynamic_complexity, _ = es.DynamicComplexity()(audio)
        acousticness = min(max(float(dynamic_complexity) / 10.0, 0.0), 1.0)

        # Valence (A helyreállított Danceability és Energy átlaga)
        valence = min(max((danceability + energy) / 2.0, 0.0), 1.0)

        # 4. Eredmény összeállítása és kiíratása
        result = {
            "valence": round(valence, 4),
            "energy": round(energy, 4),
            "danceability": round(danceability, 4),
            "acousticness": round(acousticness, 4),
            "normalizedBpm": round(normalized_bpm, 4)
        }

        print("--- ESSENTIA ELEMZÉS EREDMÉNYE ---", flush=True)
        print(result, flush=True)

        return result

    except Exception as e:
        print(f"Elemzési hiba: {e}", flush=True)
        raise HTTPException(status_code=500, detail=str(e))
    finally:
        if os.path.exists(tmp_path):
            os.remove(tmp_path)