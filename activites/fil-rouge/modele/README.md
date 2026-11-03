# Gabarit de scénario

Copiez ce dossier pour créer un nouveau [fil rouge](../README.md) : renommez-le (par exemple `activites/fil-rouge/mon-scenario/`), remplacez tout ce qui est entre `<…>`, et écrivez une page par phase à partir de [phase.md](phase.md). Le [catalogue de services](../services.md) liste des services possibles par catégorie ; l'[exemple d'équipe](equipe.md) montre une répartition. Un gabarit rempli de bout en bout est dans le [fil rouge libre](../libre/README.md#un-exemple-rempli-festival).

## Identité du scénario

| | |
|---|---|
| Nom | `<nom du scénario>` |
| Décor | `<en une phrase : qui sont les nœuds, qu'échangent-ils ?>` |
| Racine de topics | `<racine>` (par exemple `<racine>/presence/<nœud>`, `<racine>/time`, `<racine>/log/<nœud>`) |
| Vocabulaire | un nœud s'appelle ici `<boulangerie, maison, usine, station…>` |
| Types de messages ajoutés | `<type>` → schéma `<fichier>.schema.json` (voir le [contrat de base](../../contrats/README.md#extension-par-scenario)) |

## Services choisis

| Catégorie | Service retenu | Équipe |
|---|---|---|
| `<catégorie du catalogue>` | `<nom du service>` | `<qui>` |

## Couverture des exigences

| Thématique | Page de phase | Exigence satisfaite | Manque |
|---|---|---|---|
| [Sérialisation](../../../thematiques/01-serialisation.md#exigences-du-fil-rouge) | `<lien>` | `<oui / partiel>` | `<à écrire>` |
| [Temps](../../../thematiques/02-temps.md#exigences-du-fil-rouge) | | | |
| [MQTT](../../../thematiques/03-mqtt.md#exigences-du-fil-rouge) | | | |
| [Microservices](../../../thematiques/04-microservices.md#exigences-du-fil-rouge) | | | |
| [Idempotence](../../../thematiques/05-idempotence.md#exigences-du-fil-rouge) | | | |
| [Transactions](../../../thematiques/06-transactions.md#exigences-du-fil-rouge) | | | |
| [Résilience](../../../thematiques/07-resilience.md#exigences-du-fil-rouge) | | | |
| [Cluster et P2P](../../../thematiques/08-cluster-p2p.md#exigences-du-fil-rouge) | | | |
| [Sécurité](../../../thematiques/09-securite.md#exigences-du-fil-rouge) | | | |
| [Supervision](../../../thematiques/10-supervision.md#exigences-du-fil-rouge) | | | |
| [Énergie](../../../thematiques/11-energie.md#exigences-du-fil-rouge) | | | |

## Évaluation

Ajoutez une annexe sur le modèle de [annexe-modele](../../../evaluation/annexes/modele.md).
