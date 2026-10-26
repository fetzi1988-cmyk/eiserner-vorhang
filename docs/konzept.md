# Konzept (Kurzfassung)

Ausführliche Fassung: <https://claude.ai/artifact/K1ib5qc9c5yMzuuK1bK7LJ>

## Fraktionen
- **NATO:** Qualität vor Masse, starke Luftwaffe und Träger, überlegene Aufklärung.
- **Warschauer Pakt:** Masse und Feuerkraft, Artillerie, U-Boote, Flugabwehr-Netze.
- **Blockfreie Front:** asymmetrisch, Guerilla und Söldner (nach dem Prototyp).

## Ressourcen
- **Nachschub** aus Depots und Sektoren: Infanterie, Gebäude.
- **Treibstoff** aus Raffinerien und Häfen: Fahrzeuge, Flugzeuge, Schiffe.
- **Einfluss** aus Siegen und Schlüsselpunkten: Generalsfähigkeiten.

## Besonderheit: DEFCON-Leiste
Starke Fähigkeiten erhöhen die Eskalation für beide Seiten. Höhere Stufe = stärkere Fähigkeiten, aber Nachteile. DEFCON 1 schaltet die Superwaffe frei.

## Fahrplan bis zum Prototyp
| Phase | Inhalt | Dauer (Schätzung) |
|---|---|---|
| 0 | Einrichtung: Repository, Unity, Git LFS, Grundgerüst | 1 Woche |
| 1 | Kamera und Steuerung | 2 Wochen |
| 2 | Einheiten und Kampf | 3 Wochen |
| 3 | Pathfinding (Flowfields) und Formationen | 3 Wochen |
| 4 | Wirtschaft und Basisbau | 3 Wochen |
| 5 | Luft und See | 3 Wochen |
| 6 | Gegner-KI | 2–3 Wochen |
| 7 | Fog of War, Oberfläche, Minikarte | 2 Wochen |

## Technische Grundregeln
1. Spieleraktionen sind Befehle (`GameCommand`) mit Ausführungstakt. Spieler, KI und Netzwerk nutzen denselben Weg.
2. Die Simulation läuft mit festen 15 Takten pro Sekunde (`SimulationClock`), getrennt von der Grafik.
3. Spiellogik in `Scripts/Core` rechnet mit ganzen Zahlen (Positionen in Zentimetern) und hat keine Unity-Abhängigkeit. So bleibt sie deterministisch und Lockstep-Multiplayer ist später möglich.
4. Einheitenwerte stehen in Daten (ScriptableObjects), nicht im Code.
