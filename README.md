# FBMon

## FlitzMonitor

ist eine Fritz Box Monitor Windows App
(z.Zt. mit fixer deutsch-englischer Textmischung)


**Idea**:
While in homeoffice, I want to see on the Windows-Desktop as quick tray tooltip who is actually calling,
in order to decide, to pick up the call or to leave it for other family members with their phones.

**Ziel**:
Während ich im Homeoffice bin, möchte ich an meinem Windows-PC als kurzen Tray-Tooltip sehen, wer gerade anruft,
um zu entscheiden, ob ich das Gespräch annehme oder es lieber anderen Familienmitgliedern an anderen Apparaten überlasse.

Zusätzlich gibt es ein Loggging (NLog) zur Fehlersuche und eine Anzeige der Gespäche, bzw. Gesprächszustände.
Verbindungsnamen werden dem Telefonbuch der Fritzbox entnommen.

Geschrieben und im Einsatz auf einer **Fritzbox 7490**.

Um diese Daten aus dem Sourcecode herauszuhalten, werden Fritzbox Zugangsdaten über die Kommandozeile übergeben.
Aufruf: 

FlitzMonitor.exe {Fritzbox-User} {Fritzbox-User-Passwort}

wobei natürlich die {} hier weggelassen werden und die beiden Parameter in "" gesetzt werden, wenn dort Leerzeichen enthalten sind.
Die Fritzbox-IP sollte in FlitzMonitor.exe.config unter "FritzAddress" eingetragen werden.
Eventuell ist es hilfreich, für das Programm einen extra Fritzbox-User anzulegen.
