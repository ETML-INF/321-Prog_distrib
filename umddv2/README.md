# UMDD v2 — Dousville, ville distribuée

> Fil rouge du module 321 (40 périodes, ~5 p./semaine → **8 séquences**).  
> Histoire de départ (entrepreneur menteur, Chad Geppetto, coopérative) : voir [UMDD.md](../UMDD.md).
>
> **POC conservé pour mémoire.** Sur la branche `umdd-v2`, le contenu vit dans [thematiques/](../thematiques/README.md), [activites/drills](../activites/drills/README.md), [activites/chat](../activites/chat/README.md), [activites/fil-rouge](../activites/fil-rouge/README.md), [activites/infra](../activites/infra/README.md) et [activites/contrats](../activites/contrats/README.md). Les liens ci-dessous vers les anciennes thématiques ne sont plus valables sur cette branche.

## Principe : un rythme, répété à chaque séquence

```
 DRILL  ──►  CHAT +1  ──►  SERVICE DOUSVILLE  ──►  CHECKPOINT
 20 min      individuel     groupe                 10 min, Forms
 sans IA     IA permise     IA permise
 papier/CLI  améliore       un microservice
             SON chat       partagé par la classe
```

- **Drill** : comprendre le mécanisme *avant* de coder. Voir [drills.md](drills.md).
- **Chat +1** : le chat (séquence 1) est le banc d'essai personnel. Chaque thème lui ajoute **une** amélioration.
- **Service Dousville** : le même mécanisme, en vrai, dans la ville (Log, Directory, Bank…).
- **IA permise si on comprend** : voir [evaluation.md](evaluation.md).

## Langage libre

On ne donne **aucun code**, seulement un [contrat](contracts/README.md) (broker, topics, enveloppe, JSON Schema).  
N'importe quel langage ayant un client MQTT convient. Règles :

1. Chaque groupe **justifie** son langage en 3 lignes (lib MQTT, empreinte, compétences d'équipe).
2. Chaque groupe fait tourner **au moins un service dans un autre langage** que celui de sa boulangerie.
3. Le `Front` C# ([activites/umdd](../activites/umdd/README.md)) reste une vitrine *optionnelle*.

→ Vivre la fallacy 8 (« le réseau est homogène ») au lieu de la réciter.

## Les 8 fallacies, mises en scène

| # | Fallacy (l'entrepreneur dit…) | Séquence qui la démolit                 |
|---|-------------------------------|-----------------------------------------|
| 1 | Le réseau est fiable          | S3, S5                                  |
| 2 | Latence nulle                 | S2, S5                                  |
| 3 | Bande passante infinie        | S1                                      |
| 4 | Le réseau est sûr             | S6                                      |
| 5 | La topologie ne change pas    | S3, S5                                  |
| 6 | Un seul administrateur        | S3, S4 (services de groupes différents) |
| 7 | Coût de transport nul         | S7                                      |
| 8 | Réseau homogène               | S1, S2 (langages mixtes)                |
| + | Tout le monde connaît l'heure | S2                                      |

## Séquences

| Seq.   | Périodes | Thème                                              | Théorie (existante)                                                                                                                                                                            | Drill  | Chat +1                           | Service Dousville               |
|--------|----------|----------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------|-----------------------------------|---------------------------------|
| **S1** | 5        | Kickoff, chat v0, sérialisation                    | [Sérialisation](../supports/Sérialisation.md) · [Thématique](../thematiques/01-serialisation.md)                                                                                               | D1     | messages typés (enveloppe + JSON) | Vitrine / Billboard             |
| **S2** | 5        | Temps & protocole (+ choix de langage)             | [NTP](../supports/ntp.md) · [MQTT](../supports/mqtt.md) · [Thém. NTP](../thematiques/02-temps.md) · [Thém. MQTT](../thematiques/03-mqtt.md)                                                      | D2, D3 | horodatage, présence/retain       | TimeService                     |
| **S3** | 5        | Microservices, Log, Annuaire (LWT)                 | [Microservice](../supports/microservice.md) · [Thématique](../thematiques/04-microservices.md)                                                                                                  | D4     | « X est mort » (LWT)              | LogService, Directory           |
| **S4** | 5        | Idempotence & transactions                         | [Idempotence](../supports/idempotence.md) · [Transaction](../supports/transaction.md) · [Thématique](../thematiques/05-idempotence.md)                                                         | D5, D6 | déduplication par `id`            | Bank, Coop, Fair Trade          |
| **S5** | 5        | Résilience & cluster                               | [Design for failure](../supports/design4failure.md) · [Cluster](../supports/cluster.md) · [P2P](../supports/P2P.md) · [Thém. Résilience](../thematiques/07-resilience.md)                      | D7, D8 | reconnexion + backoff             | Directory en cluster            |
| **S6** | 5        | Sécurité                                           | [Confidentialité](../supports/confidentialite.md) · [Réseau de confiance](../supports/reseau-confiance.md) · [Sécurité](../supports/securite.md) · [Thématique](../thematiques/09-securite.md) | D9     | DM chiffrés, messages signés      | transferts signés               |
| **S7** | 5        | Supervision & énergie (3 p.) + vote du défi (2 p.) | [Puissance et énergie](../supports/Puissance%20et%20énergie.md) · [Thém. Monitoring](../thematiques/10-supervision.md) · [Thém. Énergie](../thematiques/11-energie.md)         | D10    | heartbeat                         | Supervision · lancement du défi |
| **S8** | 5        | Défi voté (2 p.) + épreuve (3 p.)                  | —                                                                                                                                                                                              | —      | —                                 | Défi · épreuve                  |

Les durées sont indicatives ; les séquences ne sont pas des semaines à la lettre.

## Défi voté (S7–S8)

La classe vote, parmi 3 propositions, un scénario *réel* à plaquer sur la ville terminée. Exemples (à affiner) :

- **Four connecté** : capteurs de température, alertes, batterie limitée → énergie, QoS, monitoring.
- **Livraison** : positions mobiles, perte de réseau, reprise → résilience, idempotence.
- **Black Friday** : pic de charge, faillite de la banque → backoff, cluster, transaction.

Critère : l'équipe doit nommer **quelles fallacies** le scénario fait ressurgir avant de coder.

## Filet de sécurité

Un service partagé non livré par un groupe ne doit pas bloquer la classe : le prof fournit un service de référence (stub) à chaque séquence. Les tuer volontairement fait partie des exercices (S5).

## Contenu du dossier

- [contracts/](contracts/README.md) : contrat (sans code)
- [drills.md](drills.md) : 10 drills
- [evaluation.md](evaluation.md) : pistes code / feature, grille de prompt
