# Fil rouge « El Choco »

Un **cartel de familles chocolatières** réparties sur la Suisse romande. Chaque famille tient sa maison de chocolat, avec sa zone de livraison, ses cacaos et son produit phare. Le cartel partage des services : une horloge commune, un annuaire des maisons ouvertes, un journal. Chaque affirmation sur le réseau (« il est fiable », « il est rapide »…) est l'une des [fallacies](../../../supports/fallacies.md) : **le cartel existera quand chacune aura été démentie**.

> **État : esquisse.** Seule la thématique [Sérialisation](../../../thematiques/01-serialisation.md) est écrite. Les autres phases viennent avec les séances, à partir de [phase.md](../modele/phase.md).

## Identité du scénario

| | |
|---|---|
| Nom | El Choco |
| Décor | les maisons de chocolat d'un cartel de familles et ses services communs |
| Racine de topics | `elchoco/` (`elchoco/presence/<famille>`, `elchoco/time`, `elchoco/log/<famille>`, `elchoco/vitrine/<famille>`) |
| Vocabulaire | un nœud s'appelle une **maison** (une famille = une maison) |
| Types de messages ajoutés | `chocolaterie` → [`chocolaterie.schema.json`](chocolaterie.schema.json) |
| Contrat | le [contrat de base](../../contrats/README.md) partagé, plus l'extension ci-dessus |

## Services choisis

| Catégorie | Service retenu | Équipe |
|---|---|---|
| Vitrine / état public | [La vitrine](vitrine.md) : fiche de la maison | tous |

Les autres catégories du [catalogue](../services.md) seront choisies au fil des thématiques.

## Les six questions (provisoire)

| Question | Réponse |
|---|---|
| Nœuds et publication | maisons ; chacune publie sa vitrine (nom, zone, capital, cacaos, produit phare) |
| L'heure | péremption des lots, fermeture des boutiques |
| Ce qui change | le stock de fèves de cacao, le capital de la maison |
| Opération à deux parties | acheter des fèves : le fournisseur décrémente son stock **et** la maison est débitée |
| Qui ment | une fausse maison qui vend du « criollo » ; les prix d'achat sont confidentiels |
| S'accumule ou se dépense | stock de cacao, capital en CHF |

## Couverture des exigences

| Thématique | Page de phase | Exigence satisfaite | Manque |
|---|---|---|---|
| [Sérialisation](../../../thematiques/01-serialisation.md#exigences-du-fil-rouge) | [La vitrine](vitrine.md) | oui : `chocolaterie` a un schéma, trois entrées hostiles décrites | |
| [Temps](../../../thematiques/02-temps.md#exigences-du-fil-rouge) | | | à écrire |
| [MQTT](../../../thematiques/03-mqtt.md#exigences-du-fil-rouge) | | | à écrire |
| [Microservices](../../../thematiques/04-microservices.md#exigences-du-fil-rouge) | | | à écrire |
| [Idempotence](../../../thematiques/05-idempotence.md#exigences-du-fil-rouge) | | | à écrire |
| [Transactions](../../../thematiques/06-transactions.md#exigences-du-fil-rouge) | | | à écrire |
| [Résilience](../../../thematiques/07-resilience.md#exigences-du-fil-rouge) | | | à écrire |
| [Cluster et P2P](../../../thematiques/08-cluster-p2p.md#exigences-du-fil-rouge) | | | à écrire |
| [Sécurité](../../../thematiques/09-securite.md#exigences-du-fil-rouge) | | | à écrire |
| [Supervision](../../../thematiques/10-supervision.md#exigences-du-fil-rouge) | | | à écrire |
| [Énergie](../../../thematiques/11-energie.md#exigences-du-fil-rouge) | | | à écrire |

## Phases

- [Sérialisation · La vitrine](vitrine.md)
