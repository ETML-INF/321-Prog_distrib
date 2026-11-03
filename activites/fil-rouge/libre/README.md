# Fil rouge libre

Le groupe invente son propre scénario. Il utilise le même [contrat de base](../../contrats/README.md), le même [labo](../../infra/README.md), les mêmes [drills](../../drills/README.md) et le même [chat](../../chat/README.md) que les autres : seuls le décor et les services changent.

Ce qu'on vous demande n'est pas d'inventer une histoire, mais de **construire un décor dans lequel chaque [thématique](../../../thematiques/README.md) a une raison d'exister**. Le [contrat de scénario](../README.md#contrat-de-scenario) dit ce que le décor doit contenir ; cette page vous aide à le trouver, à le cadrer et à l'avancer séance après séance.

## 1. Trouver un décor

Un bon décor répond oui à ces six questions. Posez-les à voix haute pendant la proposition.

| Question | Pourquoi (thématique) |
|---|---|
| Qui sont les **nœuds** (au moins 6, un par personne) et qu'est-ce qu'ils publient d'eux-mêmes ? | Sérialisation, MQTT |
| Qu'est-ce qui dépend de l'**heure** ? Que se passe-t-il si deux nœuds ne sont pas d'accord ? | Temps |
| Qu'est-ce qui **change** quand un message arrive (un solde, un stock, une file, un état) ? Que se passe-t-il s'il arrive deux fois ? | Idempotence |
| Quelle opération concerne **deux parties** et ne doit pas être faite à moitié ? | Transactions |
| Qui peut **mentir** ou se faire passer pour un autre ? Qu'est-ce qui est confidentiel ? | Sécurité |
| Qu'est-ce qui **s'accumule ou se dépense** dans le temps, et que mesure-t-on pour savoir si tout va bien ? | Supervision, Énergie |

Si une question n'a pas de réponse naturelle, ajustez le décor plutôt que de forcer l'exigence. Si elle n'en a vraiment pas, notez-le comme un manque dans le tableau de couverture : c'est acceptable, caché ça ne l'est pas.

**À éviter**

- Un décor qui n'est qu'un renommage d'un des scénarios existants (une boulangerie devenue boucherie ne vous apprend rien de nouveau).
- Un décor qui demande des données réelles, un compte externe ou un matériel que seule une personne possède.
- Un décor où un seul service fait tout.

## 2. Cadrer la taille

Un scénario tenable, c'est :

- **4 à 6 services**, un propriétaire par service (voir [l'exemple d'équipe](../modele/equipe.md)) ;
- **1 type de message métier** au départ (un second plus tard, pas avant la thématique qui le demande) ;
- **chaque service testable seul**, en jouant ses voisins à la main avec le labo ;
- **aucun service indispensable à tous** au point que sa panne bloque la classe : c'est la raison du filet ci-dessous.

Le [catalogue de services](../services.md) liste des services possibles. Choisissez une option par catégorie dont vous avez besoin, pas toutes.

## 3. Démarche

1. **Proposer.** Chaque personne ou sous-groupe présente un décor en quelques lignes et répond aux six questions.
2. **Voter.** La classe retient un scénario (vote), avec l'enseignant.
3. **Choisir une racine de topics**, qui ne doit pas entrer en conflit avec `chat/`, `lab/` ni celles des autres scénarios.
4. **Partir du [gabarit](../modele/README.md)** : copier le dossier, remplir l'identité, choisir les services dans le [catalogue](../services.md) (en ajoutant au besoin une catégorie, en disant quelle exigence elle remplit).
5. **Valider** avec la checklist ci-dessous.
6. **Écrire les phases au fil du module** (voir les jalons).

## 4. Jalons

Une page de phase par thématique, écrite **au moment où la thématique est abordée**, à partir de [phase.md](../modele/phase.md). L'ordre est celui du [planning](../../../planning/README.md) suggéré ; si votre classe le change, suivez la thématique, pas le numéro de séquence.

| Quand | Ce que le scénario doit avoir à la fin de la séance | Preuve minimale |
|---|---|---|
| Mise en route (séq. 1) | proposition votée, racine choisie, identité du gabarit remplie, services choisis et propriétaires nommés | la page d'accueil du scénario existe et se construit |
| [Sérialisation](../../../thematiques/01-serialisation.md#exigences-du-fil-rouge) | le type métier et son schéma, dans le dossier du scénario | un message valide et trois entrées hostiles ignorées |
| [Temps](../../../thematiques/02-temps.md#exigences-du-fil-rouge), [MQTT](../../../thematiques/03-mqtt.md#exigences-du-fil-rouge) | une source d'heure, un flux retenu, un flux d'événements | un nœud tardif voit l'état ; décalage visible avec `--clock-skew 30` |
| [Microservices](../../../thematiques/04-microservices.md#exigences-du-fil-rouge) | **le contrat de l'annuaire et du journal**, relu par la classe **avant** de coder | un nœud tué disparaît de la liste en moins de 10 s |
| [Idempotence](../../../thematiques/05-idempotence.md#exigences-du-fil-rouge), [Transactions](../../../thematiques/06-transactions.md#exigences-du-fil-rouge) | l'opération qui change un état partagé et celle à deux parties, contrats écrits | doublon sans effet ; panne au milieu sans demi-état |
| [Résilience](../../../thematiques/07-resilience.md#exigences-du-fil-rouge), [Cluster et P2P](../../../thematiques/08-cluster-p2p.md#exigences-du-fil-rouge) | une dépendance qui tombe et un service en plusieurs instances | reprise seule au retour ; perte d'un membre sans perte de service |
| [Sécurité](../../../thematiques/09-securite.md#exigences-du-fil-rouge) | amendement `v: 2` décidé avec la classe, puis appliqué service par service | usurpation et rejeu refusés |
| [Supervision](../../../thematiques/10-supervision.md#exigences-du-fil-rouge), [Énergie](../../../thematiques/11-energie.md#exigences-du-fil-rouge) | un `hb` par service, un moniteur, un bilan | silence détecté au bon moment ; bilan juste sous pertes |
| Défi | un défi voté, avec ses fallacies | critère de réussite écrit **avant** de coder |

Si vous avez du retard, ne sautez pas une page : écrivez la DoD, marquez ce qui manque dans le tableau de couverture, avancez. Un manque noté vaut mieux qu'une page absente.

## 5. Filet de sécurité

Un décor libre n'a pas de service de référence fourni. Prévoyez-le :

- l'enseignant (ou un groupe volontaire) fournit une **version minimale** de chaque service partagé (temps, annuaire, paiement) dès qu'un contrat est fusionné ;
- un service de référence peut être arrêté par l'enseignant pour un exercice de résilience ;
- si un groupe n'a pas livré à la fin d'une séance, la classe continue sur la version de référence.

À convenir avec l'enseignant au vote, pas après la première panne.

## Checklist de validation

- [ ] la racine de topics est choisie et déclarée ;
- [ ] les six questions de la section 1 ont une réponse, ou un manque noté ;
- [ ] chaque thématique a son [exigence](../README.md#contrat-de-scenario) couverte, ou un manque noté dans le tableau de couverture ;
- [ ] un service fournit le temps, un autre change un état partagé sans doublon ni demi-succès, un est observé, et un peut mourir sans tuer les autres ;
- [ ] les types de messages ajoutés ont un schéma qui valide contre l'[enveloppe](../../contrats/envelope.schema.json) ;
- [ ] un filet de sécurité est convenu avec l'enseignant ;
- [ ] une annexe d'évaluation existe (voir [le gabarit](../../../evaluation/annexes/modele.md)).

## Un exemple rempli : Festival

Pour montrer **la forme**, pas pour être copié. Le décor est inventé pour cette page et n'est pas l'un des fils rouges existants.

### Le décor

Un festival de musique de trois jours. Chaque **stand** (buvette, scène, merch, accueil) est un nœud. Des services partagés tiennent le festival : une horloge commune (les concerts commencent à l'heure), une billetterie à jetons, un annuaire des stands ouverts, un moniteur de santé.

### Les six questions

| Question | Réponse |
|---|---|
| Nœuds et publication | stands ; chacun publie sa fiche (nom, type, file d'attente) |
| L'heure | horaires des concerts et fermeture des buvettes ; deux horloges qui divergent = un concert « commencé » pour l'un seulement |
| Ce qui change | le solde de jetons d'un visiteur ; reçu deux fois, il est débité deux fois |
| Opération à deux parties | acheter une boisson : le stand décrémente son stock **et** le visiteur est débité |
| Qui ment | un faux stand qui encaisse ; les achats sont confidentiels |
| S'accumule ou se dépense | jetons en circulation, stock de bière, flux de visiteurs |

### Identité (gabarit rempli)

| | |
|---|---|
| Nom | Festival |
| Décor | les stands d'un festival et ses services communs |
| Racine de topics | `festival/` (`festival/presence/<stand>`, `festival/time`, `festival/log/<stand>`) |
| Vocabulaire | un nœud s'appelle un **stand** |
| Types de messages ajoutés | `stand` → `stand.schema.json` ; `purchase` → `purchase.schema.json` |

### Services choisis

| Catégorie | Service retenu | Équipe |
|---|---|---|
| Source de temps | Horloge du festival | A |
| Annuaire | Plan des stands ouverts | B |
| Paiement | Billetterie à jetons | C |
| Échange | Achat d'une boisson (stock + débit) | C, D |
| Supervision | Régie (moniteur de battements) | D |
| Simulateur d'événements | Flux de visiteurs | E |
| Vitrine / état public | Fiche de stand | tous |

### Un message du type ajouté

Un achat est une enveloppe du [contrat de base](../../contrats/README.md#enveloppe) dont le `type` est `purchase`. Il se rejoue sans effet grâce à son `id` :

```json
{"v":1,"id":"6c1f6a9e-9b4f-4c44-8e3f-1f3d9a7c2b10","from":"visiteur-42","to":"buvette-nord",
 "ts":"2026-11-03T20:15:30Z","type":"purchase",
 "payload":{"item":"biere","qty":1,"tokens":4}}
```

### Couverture des exigences (extrait)

| Thématique | Exigence satisfaite | Manque |
|---|---|---|
| Sérialisation | `stand` et `purchase` ont un schéma | trois entrées hostiles à formaliser |
| Temps | horloge du festival ; concerts annoncés à l'heure | mode dégradé si l'horloge tombe |
| Idempotence | un `purchase` rejoué ne débite pas deux fois (clé = `id`) | borne de la mémoire de déduplication |
| Transactions | stock + débit en deux étapes avec compensation | panne entre les deux étapes à provoquer |
| Énergie | bilan jetons en circulation | pertes de messages à simuler |

### Une page de phase (extrait, Idempotence)

> **Mission.** La billetterie reçoit des demandes d'achat. Elle débite le visiteur **une seule fois** même si la demande arrive deux fois, refuse un solde insuffisant, et répond `accepté`, `refusé` ou `doublon`.
>
> **DoD.** Je renvoie le même `purchase` trois fois : un seul débit. Je tue la billetterie entre le débit et la réponse, je la relance : ni double débit ni achat perdu.
>
> **À décider en groupe.** Combien de temps retient-on les `id` déjà traités ? (fallacy 1, un message peut arriver très tard)

Les pages de phase suivent toutes ce modèle : voir [phase.md](../modele/phase.md).
