# Feature Specification: Jass MCP API

**Feature Branch**: `[001-jass-mcp-api]`

**Created**: 2026-10-08

**Status**: Draft

**Input**: User description: "Ausgangslage: Wir wollen eine ASP.Net Core Web API erstellen, die auf eine bestehende Datenbank zugreift und die Daten als MCP Server zur Verfügung stellt. Features: Web API Anmeldung via EntraID; MCP Server für Jass-Fragen. Offen: Es gibt eine bestehende Blazor WebAssembly App mit der Domain Logik. Diese auch mitnehmen?" Ergänzung: MVP-Umfang A - Fragen zu gespeicherten Daten, insbesondere Spieler und Spielrunden.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Spieler und gespeicherte Jass-Runden abfragen (Priority: P1)

Ein angemeldeter Anwender nutzt einen MCP-kompatiblen Client, um Spieler zu finden und gespeicherte Spielrunden eines Spielers einzusehen. Der Client kann Runden nach Gegner, Zeitraum und Trainingslager eingrenzen und erhält zu passenden abgeschlossenen Runden Teams, Resultate und Trumpfrunden. Die natürliche Sprachinterpretation übernimmt der Client; der Dienst stellt dafür klar beschriebene, strukturierte Datenfunktionen bereit.

**Why this priority**: Der Zugriff auf bestehende Spielerdaten und Spielrunden ist der vereinbarte MVP-Nutzen und wird durch vorhandene Daten- und Abfragekonzepte unterstützt.

**Independent Test**: Ein autorisierter MCP-Client kann Spieler auflisten, eine abgeschlossene Runde eines Spielers mit ihren Teams und Ergebnissen abrufen und eine Suche mit Filtern wiederholen. Bei keinem Treffer erhält er eine eindeutige leere Ergebnismenge.

**Acceptance Scenarios**:

1. **Given** der MCP-Client ist autorisiert und Spieler sind gespeichert, **When** er die Spielerabfrage aufruft, **Then** erhält er die vorhandenen Spieler-IDs und Namen.
2. **Given** ein Spieler hat abgeschlossene und gespeicherte Runden, **When** der Client Runden dieses Spielers abfragt, **Then** erhält er passende Runden mit Startdatum, Teams, Spielern, Resultaten und Trumpfrunden.
3. **Given** Gegner-, Datums- oder Trainingslagerfilter werden angegeben, **When** der Client die Rundenabfrage ausführt, **Then** enthält die Ergebnismenge nur Runden, die allen angegebenen Filtern und dem vereinbarten Abschlusskriterium entsprechen, einschließlich Teams, Resultaten und Trumpfrunden.
4. **Given** keine gespeicherten Runden passen zu Spieler und Filtern, **When** der Client die Rundenabfrage ausführt, **Then** erhält er eine erfolgreiche, leere Ergebnismenge und keine erfundenen Einträge.
5. **Given** der bestehende Datenbestand ist vorübergehend nicht erreichbar, **When** eine Spieler- oder Rundenabfrage ausgeführt wird, **Then** erhält der Client einen verständlichen Fehler und keine als gültig dargestellten Ersatzdaten.
6. **Given** ein MCP-Client sendet eine ungültige Spielerkennung oder einen ungültigen Datumsbereich, **When** er die Abfrage ausführt, **Then** erhält er einen verständlichen Validierungsfehler und die Datenquelle wird nicht mit ungültigen Parametern abgefragt.

---

### User Story 2 - Zugriffe sicher authentifizieren (Priority: P1)

Ein berechtigter Anwender verbindet einen API- oder MCP-Client mit dem Dienst. Der Dienst prüft die Anmeldung über die Organisationsidentität und die erforderliche Berechtigung, bevor geschützte Spielerdaten oder Spielrunden ausgegeben werden.

**Why this priority**: Spielerdaten und Spielverläufe dürfen nur für angemeldete und zugelassene Nutzer verfügbar sein.

**Independent Test**: Zugriffe mit gültiger Entra-ID-Anmeldung und erforderlicher Berechtigung werden akzeptiert; nicht angemeldete oder nicht berechtigte Zugriffe erhalten keine geschützten Daten.

**Acceptance Scenarios**:

1. **Given** der Anwender verfügt über eine gültige Entra-ID-Anmeldung und Berechtigung, **When** er eine geschützte MCP-Abfrage stellt, **Then** wird die Anfrage authentifiziert und verarbeitet.
2. **Given** der Anwender ist nicht angemeldet oder legt ungültige Anmeldedaten vor, **When** er eine geschützte Anfrage stellt, **Then** wird der Zugriff abgewiesen und es werden keine geschützten Daten zurückgegeben.
3. **Given** die Anmeldung ist gültig, aber die erforderliche Berechtigung fehlt, **When** der Anwender eine geschützte Anfrage stellt, **Then** wird der Zugriff abgewiesen.

---

### User Story 3 - Bestehende Daten und Jass-Domain konsistent wiederverwenden (Priority: P1)

Verantwortliche möchten, dass MCP-Abfragen dieselben Spieler, gespeicherten Runden und fachlichen Jass-Ergebnisse verwenden wie die bestehende Anwendung, statt parallele Daten oder Berechnungen einzuführen. Die vorhandene Anwendungsoberfläche ist nicht Teil des API-Hosts.

**Why this priority**: Eine einzige maßgebliche Daten- und Regellogik verhindert widersprüchliche Resultate und doppelte Pflege.

**Independent Test**: Für eine repräsentative Auswahl gespeicherter Spieler und Runden stimmen API/MCP-Ergebnisse mit den vorhandenen Domain-Abfragen und fachlichen Berechnungen überein; der API-Host läuft ohne Blazor-Oberfläche oder browsergebundene Dienste.

**Acceptance Scenarios**:

1. **Given** dieselben gespeicherten Daten und Filter werden von der bestehenden Anwendung und dem Dienst verwendet, **When** eine Spieler- oder Rundenabfrage ausgeführt wird, **Then** stimmen IDs, Namen, Teams, Runden und gespeicherte Resultate überein.
2. **Given** eine Runde enthält Jass-Berechnungen im bestehenden Domainmodell, **When** der Dienst diese Runde ausgibt, **Then** verwendet er die bestehenden Regeln und erfindet oder dupliziert keine abweichenden Regeln.
3. **Given** die bestehende Anwendung enthält UI- oder browsergebundene Funktionen, **When** der Dienst gestartet wird, **Then** sind diese Funktionen keine Voraussetzung für API oder MCP.
4. **Given** der Dienst verarbeitet MCP-Abfragen, **When** er auf die bestehende Datenbank zugreift, **Then** führt er ausschließlich lesende Datenoperationen aus.

---

### Edge Cases

- Eine Spielerkennung existiert nicht.
- Ein Spieler hat keine abgeschlossenen Runden.
- Filter ergeben eine leere Ergebnismenge.
- Der Zeitraum beginnt nach seinem Enddatum.
- Der Datenbestand enthält unvollständige oder fachlich ungültige Team-/Rundendaten.
- Die Anmeldung ist gültig, aber die erforderliche Berechtigung fehlt.
- Die bestehende Datenquelle ist nicht verfügbar.
- Ein MCP-Client wiederholt eine lesende Abfrage oder sendet eine fehlerhafte Anfrage.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Ein autorisierter MCP-Client MUST gespeicherte Spieler mit ihrer stabilen Spielerkennung und ihrem Namen abrufen können.
- **FR-002**: Ein autorisierter MCP-Client MUST abgeschlossene Spielrunden für eine angegebene Spielerkennung abrufen können.
- **FR-003**: Die Rundenabfrage MUST optionale Filter für Gegner, Startdatum ab, Startdatum bis und Trainingslagerstatus unterstützen.
- **FR-004**: Jedes Rundenresultat MUST Startdatum, Punktwert, Trainingslagerstatus, beide Teams mit je zwei Spielern, sowie die gespeicherten Trumpfrunden mit Trumpftyp, Punktmodifikator und nullable Resultaten/Kennzeichen beider Teams enthalten.
- **FR-005**: Rundenabfragen MUST nur abgeschlossene Runden mit gültiger, vom bestehenden Domainmodell unterstützter Teamstruktur zurückgeben.
- **FR-006**: Eine gültige Abfrage ohne Treffer MUST eine leere Ergebnismenge zurückgeben, ohne fehlende Daten als Fakten zu erfinden.
- **FR-007**: Der Dienst MUST verständliche Validierungsfehler für unbekannte Spielerkennungen und ungültige Filterwerte liefern.
- **FR-008**: Der Dienst MUST bei Ausfällen der Datenquelle einen verständlichen operativen Fehler liefern und diesen nicht als leere Ergebnismenge darstellen.
- **FR-009**: Der Dienst MUST geschützte API- und MCP-Abfragen über Entra ID authentifizieren und die für den Zugriff konfigurierte Berechtigung prüfen.
- **FR-010**: Der Dienst MUST unberechtigte Anfragen abweisen, ohne geschützte Spielerdaten oder Rundenresultate offenzulegen.
- **FR-011**: Der Dienst MUST die bestehenden Domain- und Datenzugriffskomponenten der Jass-Anwendung als maßgebliche Quelle verwenden.
- **FR-012**: Der Dienst MUST für den MCP-Datenzugriff ausschließlich lesende Operationen ausführen.
- **FR-013**: Der Dienst MUST die bestehende Blazor-Oberfläche und browsergebundene UI-Dienste nicht als Voraussetzung für API/MCP verwenden.
- **FR-014**: Die Interpretation natürlicher Sprache MUST beim MCP-Client verbleiben; der Dienst MUST nur die explizit beschriebenen strukturierten Datenfunktionen anbieten.
- **FR-015**: Der Dienst MUST keine beliebigen SQL-Abfragen, freie Datenbankexploration oder Schreiboperationen als MCP-Funktionen anbieten.

### Key Entities *(include if feature involves data)*

- **Spieler**: Bestehender Jass-Spieler mit stabiler ID und Name.
- **Coiffeur-Spielrunde**: Gespeicherte Runde mit Startdatum, Punktwert, Trainingslager-/Optionskennzeichen, Teams und Trumpfrunden.
- **Jass-Team**: Bestehendes Team einer Runde mit Teamtyp und zwei zugeordneten Spielern.
- **Trumpfrunde**: Teilrunde mit Trumpftyp, Punktmodifikator und gespeicherten Resultaten beider Teams.
- **Abfragefilter**: Spielerkennung sowie optionale Gegnerkennung, Startdatum-Untergrenze, Startdatum-Obergrenze und Trainingslagerstatus.
- **Entra-ID-Anwenderidentität**: Verifizierte Identität mit der für den geschützten Datenzugriff erforderlichen Berechtigung.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Mindestens 95 % der Abfragen aus dem vereinbarten Abnahmesatz liefern die erwarteten Spieler oder Runden einschließlich der erwarteten Filterwirkung.
- **SC-002**: Mindestens 95 % der erfolgreichen MCP-Abfragen des Abnahmesatzes werden innerhalb von 3 Sekunden beantwortet, ausgenommen nachweisbare Verzögerungen der bestehenden Datenquelle.
- **SC-003**: 100 % der getesteten nicht angemeldeten oder unberechtigten Abfragen liefern keine geschützten Spieler- oder Rundendaten.
- **SC-004**: Mindestens 90 % der Testanwender können über einen kompatiblen MCP-Client Spieler finden und anschließend eine gefilterte Rundenabfrage erfolgreich durchführen.
- **SC-005**: Für alle geprüften Abfragefälle können Testanwender korrekt erkennen, ob eine Rundenliste Treffer enthält oder leer ist und ob ein Datenquellenfehler stattgefunden hat.
- **SC-006**: Die Integrationstests weisen für alle getesteten MCP-Funktionen null Datenbank-Schreiboperationen nach.

## Assumptions

- Die bestehende SQL-Server-Datenbank bleibt maßgeblich; Tabellen und Daten werden nicht migriert oder ersetzt.
- Der Dienst ist zur Laufzeit und beim Build eigenständig; seine Datenabbildung und die für die beiden MCP-Werkzeuge notwendigen Jass-Berechnungen befinden sich im API-Projekt.
- Die im MVP vorgesehenen Inhalte sind Spielerlisten und abgeschlossene Coiffeur-Spielrunden einschließlich Teams und Resultaten; allgemeine Jass-Regelfragen und freie natürlichsprachliche Dateninterpretation sind nicht Teil des MVP.
- Der MCP-Client interpretiert natürliche Sprache und ruft strukturierte Werkzeuge mit Spieler-IDs und Filtern auf.
- Eine gültige Rundenabfrage ohne Treffer ist erfolgreich und liefert eine leere Ergebnismenge.
- Fachliche Jass-Regeln und Datentransformationen werden aus der bestehenden Anwendung wiederverwendet; Blazor-Oberfläche und browsergebundene UI-Dienste werden nicht in den API-Host übernommen.
- Die konkrete Entra-ID-Berechtigung und ein sicherer Testdatenbankzugang werden für die Zielumgebung bereitgestellt und nicht als Geheimnisse in Quelldateien abgelegt.
