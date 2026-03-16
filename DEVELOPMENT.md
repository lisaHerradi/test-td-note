# Suivi du développement du TD noté

Ce document décrit l’état d’avancement du **squelette** du TD noté (FilmApi) et ce qui reste à faire ou à affiner. Il permet de reprendre le développement ou d’adapter le sujet.

Référence : **PLAN.md** (contexte, objectifs, structure, consignes, critères de notation).

---

## État actuel (squelette livré)

### Solution et projets

- [x] **FilmApi.slnx** — Solution .NET 10 au format .slnx.
- [x] **AppHost** — Projet Aspire avec :
  - Conteneur **MongoDB** (mongo:7), port 27017, volume persistant.
  - Projet **FilmApi** (film-api) avec connexion MongoDB injectée.
  - Projets **SeedFilms** en démarrage explicite : **seed-50k** (50000), **seed-500k** (500000).
  - Conteneur **InfluxDB** 2.7-alpine (org k6, bucket k6, token k6-influxdb-token).
  - Conteneur **Grafana** 11.3.0 avec bind mount `grafana/provisioning` (datasource InfluxDB + dashboards k6).
- [x] **src/FilmApi** — API ASP.NET Core (minimal API) :
  - Modèle **Film** imbriqué : Réalisateur (Director), Genres, Acteurs, PaysProduction (Country).
  - Endpoints : `GET /films` (paginé), `GET /films/{id}`, `POST /films`.
  - Persistance **MongoDB** (driver officiel), collection `films`, base `filmapi`.
- [x] **scripts/SeedFilms** — Console app : argument 50000 ou 500000 ; vidage de la collection puis insertion par lots (données allégées : un réalisateur, un genre par film).
- [x] **scripts/load-test/** et **scripts/spike-test/** — Scripts k6 pour `/films` (config dans `scripts/lib/config.js`) ; exécutions 50k et 500k via variable d’environnement `TOTAL_ITEMS`.
- [x] **grafana/provisioning** — Datasource InfluxDB-k6 et dashboard k6 Load Testing (aligné sur iut-performance-testing).
- [x] **tests/FilmApi.Tests** — Projet de tests avec :
  - **Verify.Xunit** déjà référencé.
  - **MongoFixture** (Testcontainers.MongoDb), **FilmApiAppFactory** (WebApplicationFactory + connexion MongoDB).
  - **FilmServiceUnitTests** : 2 tests unitaires (mock repository) — construction manuelle des objets (à refactoriser en AAA + builders).
  - **FilmDetailSnapshotTests** : 1 test avec longue série d’`Assert.Equal` sur un Film complexe (à refactoriser en snapshot Verify).
  - **FilmApiIntegrationTests** : 2 tests d’intégration (POST 201, GET après POST) déjà implémentés.
- [x] **README.md** — Consignes complètes pour les étudiants (prérequis, structure, workflow perf, consignes par partie, répartition du temps, commandes).

---

## Décisions techniques prises

- **MongoDB** : driver C# officiel (MongoDB.Driver), pas d’ORM. Connexion via `ConnectionStrings:mongodb` ou `MongoDb:ConnectionString` / `MongoDb:DatabaseName`.
- **AppHost** : connexion MongoDB en `mongodb://localhost:27017` (port fixe) pour les processus .NET (FilmApi, SeedFilms) ; les conteneurs sont sur le même réseau.
- **Seeds** : modèle allégé (un réalisateur, un genre par film) pour garder des temps d’insertion raisonnables à 50k/500k.
- **Tests de perf** : pas d’exécutables Aspire dédiés pour k6 ; les scripts sont dans `scripts/load-test` et `scripts/spike-test` ; l’étudiant lance k6 en ligne de commande avec `BASE_URL` et `TOTAL_ITEMS` (documenté dans le README). InfluxDB et Grafana sont prêts pour recevoir les métriques k6 si l’étudiant configure l’output InfluxDB dans k6.

---

## À faire / à affiner (optionnel)

- [ ] **Exécutables Aspire pour k6** : si souhaité, ajouter 4 exécutables (load-50k, load-500k, spike-50k, spike-500k) qui lancent k6 avec les bonnes variables (nécessite k6 installé ou conteneur k6).
- [ ] **Vérifier** que Grafana dans le conteneur résout bien `influxdb:8086` (réseau Docker Aspire) ; sinon adapter l’URL dans `grafana/provisioning/datasources/influxdb.yml` (ex. host.docker.internal ou nom de ressource Aspire).
- [ ] **Build et tests** : `dotnet build` réussit. `dotnet test tests/FilmApi.Tests` nécessite Docker (Testcontainers). Les tests unitaires et le test snapshot passent ; en cas de 500 sur les tests d’intégration (POST /films), vérifier la configuration MongoDB injectée (ConnectionStrings:mongodb) et la sérialisation JSON du body (CreateFilmRequest avec objets imbriqués).
- [ ] **Création des builders** : le squelette ne contient pas encore les classes FilmBuilder / DirectorBuilder / ActorBuilder ; c’est **volontaire** (consigne TD : les introduire et refactoriser au moins 3 tests).

---

## Changelog (résumé)

- **Initial** : création de la structure complète (solution, AppHost, FilmApi, SeedFilms, scripts k6, grafana provisioning, tests avec MongoFixture, WebApplicationFactory, tests unitaires et intégration, README, DEVELOPMENT.md).
