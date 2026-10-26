# Eiserner Vorhang – Hinweise für Claude

- Antworte dem Nutzer auf Deutsch, einfach erklärt (Anfänger in Unity und Git).
- Engine: Unity 6 LTS (6000.0.x), C#. Der Nutzer testet in Unity, Claude kann Unity nicht selbst ausführen.
- `Assets/_Project/Scripts/Core` (Assembly `EisernerVorhang.Core`) hat `noEngineReferences: true`: kein `UnityEngine`, keine `float` im Spielzustand (nur `SimulationClock` rechnet mit Echtzeit), nur ganze Zahlen bzw. später Festkomma. Deterministisch halten.
- Unity-Anbindung gehört nach `Assets/_Project/Scripts/Game` (Assembly `EisernerVorhang.Game`).
- Jede neue Kernlogik bekommt EditMode-Tests in `Assets/_Project/Tests/EditMode`.
- Code-Kommentare und Bezeichner im Code: Bezeichner Englisch, Kommentare Deutsch.
- Keine `.meta`-Dateien von Hand erfinden; Unity erzeugt sie, der Nutzer committet sie.
- Konzept und Fahrplan: `docs/konzept.md`.
