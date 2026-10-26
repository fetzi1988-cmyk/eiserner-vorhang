# Eiserner Vorhang

Ein Echtzeit-Strategiespiel im heiß gewordenen Kalten Krieg (um 1985): Land, Luft und See auf einer Karte.
Vorbilder: C&C Generäle, Company of Heroes, Empire Earth.

- Konzept und Fahrplan: [docs/konzept.md](docs/konzept.md)
- Engine: **Unity 6 LTS** mit C#

## Projekt zum ersten Mal öffnen

Du brauchst das nur einmal zu machen.

1. **Unity Hub installieren:** <https://unity.com/download> herunterladen und installieren, mit einem kostenlosen Unity-Konto anmelden (Lizenz „Personal“).
2. **Unity 6 installieren:** Im Unity Hub links auf *Installs* → *Install Editor* → die oberste Version **Unity 6 (6000.0.x) LTS** wählen. Bei den Modulen reicht die Standardauswahl, für Windows-Builds „Windows Build Support“ anhaken.
3. **GitHub Desktop installieren:** <https://desktop.github.com> (einfacher als die Kommandozeile). Mit deinem GitHub-Konto anmelden.
4. **Repository holen:** In GitHub Desktop *File* → *Clone repository* → `eiserner-vorhang` wählen → einen Ordner aussuchen, z. B. `Dokumente/Spiele`.
5. **Git LFS:** GitHub Desktop fragt beim ersten Mal eventuell, ob Git LFS eingerichtet werden soll. Mit *Ja* bestätigen. Darüber laufen später große Dateien wie Modelle und Texturen.
6. **Im Unity Hub öffnen:** *Projects* → *Add* → *Add project from disk* → den Ordner `eiserner-vorhang` auswählen. Falls Unity meldet, dass eine andere Version erwartet wird, einfach deine installierte Unity-6-Version auswählen und bestätigen.
7. Der erste Start dauert ein paar Minuten, Unity baut dabei den Ordner `Library` auf. Der wird nicht hochgeladen.

## Den aktuellen Stand testen

1. In Unity unten im *Project*-Fenster `Assets/_Project/Scenes` öffnen.
2. Oben *File* → *New Scene* → *Basic (Built-in)* → *Create*, dann mit *Strg+S* als `Testgelaende` im Ordner `Scenes` speichern.
3. Im *Hierarchy*-Fenster Rechtsklick → *Create Empty*, das neue Objekt `Spiel` nennen.
4. Rechts im *Inspector* auf *Add Component* klicken, `GameBootstrap` suchen und hinzufügen.
5. Oben auf ▶ (*Play*) drücken. Es erscheint ein grünes Testgelände mit 12 blauen (deinen) und 6 roten Platzhalter-Panzern.

### Steuerung

| Taste | Wirkung |
|---|---|
| W A S D, Pfeiltasten oder Maus an den Bildrand | Kamera verschieben |
| Mausrad | Zoomen |
| Q / E oder mittlere Maustaste gedrückt halten | Kamera drehen |
| Linksklick | Einheit auswählen |
| Linke Maustaste gedrückt ziehen | Auswahlrahmen |
| Shift + Klick | Zur Auswahl hinzufügen |
| Rechtsklick auf den Boden | Ausgewählte Einheiten fahren dorthin |
| X | Anhalten |
| Strg + 1 bis 9 | Auswahl als Gruppe speichern |
| 1 bis 9 | Gespeicherte Gruppe auswählen |

**Tests ausführen:** *Window* → *General* → *Test Runner* → Reiter *EditMode* → *Run All*. Alle Häkchen sollten grün sein.

## Änderungen hochladen

Wenn Unity neue Dateien angelegt hat (z. B. `.meta`-Dateien oder deine Szene), siehst du sie in GitHub Desktop. Unten links eine kurze Beschreibung eintragen, *Commit to main* und dann *Push origin* klicken.

## Ordnerstruktur

| Ordner | Inhalt |
|---|---|
| `Assets/_Project/Scripts/Core` | Spiellogik ohne Unity-Abhängigkeit: Befehle, Simulationstakt. Läuft auf allen Rechnern exakt gleich. |
| `Assets/_Project/Scripts/Game` | Verbindung zu Unity: Kamera, Eingabe, Darstellung. |
| `Assets/_Project/Tests` | Automatische Tests. |
| `Assets/_Project/Data` | Einheitenwerte als ScriptableObjects (ab Phase 2). |
| `Assets/_Project/Art`, `Audio`, `Prefabs`, `Scenes` | Grafik, Ton, Bausteine, Karten. |
| `docs` | Konzept und Pläne. |

Gekaufte Asset-Store-Pakete landen in eigenen Ordnern direkt unter `Assets/` und bleiben getrennt von unserem Code.
