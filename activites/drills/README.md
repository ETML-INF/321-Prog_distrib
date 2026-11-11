# Drills

Des exercices **guidés**, courts (≈ 20 min), **sans IA**, à faire *avant* de coder. Chacun fait toucher du doigt un mécanisme de la théorie, sur papier ou en ligne de commande, avec le résultat attendu à chaque étape.

Un drill réussi = vous pouvez expliquer à voix haute **ce qui s'est passé et pourquoi**. C'est ce qui vous permettra ensuite de relire (et juger) du code généré.

## Avant de commencer

- Les commandes se lancent depuis `activites/infra` ([labo Docker](../infra/README.md)) : `docker compose up -d`.
- Un terminal par rôle (Alice, Bob, observateur) pour les drills en ligne de commande.
- Les blocs **Solution** sont repliés : essayez d'abord.

## Liste

| Drill | Thématique | Thème | Fallacy | Format |
|---|---|---|---|---|
| [D01](D01-serialisation.md) | [Sérialisation](../../thematiques/01-serialisation.md) | Sérialisation : combien ça pèse, qu'est-ce qui casse | 3, 8 | papier + validateur |
| [D02](D02-temps.md) | [Temps](../../thematiques/02-temps.md) | Temps : trier des messages quand les horloges mentent | heure | Docker, papier |
| [D03](D03-qos-retain.md) | [MQTT](../../thematiques/03-mqtt.md) | MQTT : QoS, session, retain | 1, 2 | CLI |
| [D04](D04-mort-noeud.md) | [Microservices](../../thematiques/04-microservices.md) | Mort d'un nœud : LWT et keep-alive | 1, 5 | CLI + pannes |
| [D05](D05-idempotence.md) | [Idempotence](../../thematiques/05-idempotence.md) | Idempotence : le solde après les doublons | 1 | papier |
| [D06](D06-transaction.md) | [Transactions](../../thematiques/06-transactions.md) | Transaction : Fair Trade quand la banque plante | 1, 6 | papier |
| [D07](D07-retry.md) | [Résilience](../../thematiques/07-resilience.md) | Retry : backoff et jitter aux dés | 1, 2 | papier + dés |
| [D08](D08-cluster.md) | [Cluster et P2P](../../thematiques/08-cluster-p2p.md) | Cluster : qui répond (et que faire si deux répondent) | 5, 6 | papier + CLI |
| [D09](D09-securite.md) | [Sécurité](../../thematiques/09-securite.md) | Sécurité : signature, chiffrement, rejeu | 4 | papier + CLI |
| [D10](D10-monitoring-energie.md) | [Supervision](../../thematiques/10-supervision.md) | Monitoring et énergie | 7 | CLI + calcul |

Théorie transversale : [les 8 fallacies](../../supports/fallacies.md).
