# Évaluation — grille du parcours

L'IA est permise **pendant le parcours** (labo, chat), pas pendant l'évaluation : on mesure la compréhension, pas la production.

## Deux types d'évaluation

| Quoi                        | Contenu                          | Support              |
|-----------------------------|----------------------------------|----------------------|
| **Checkpoints** (plusieurs) | **Théorie uniquement**           | Forms, 10 min        |
| **Séquence finale** (une)   | Questions *code* et *story*      | Papier **ou** oral   |

## Checkpoints : théorie
Forms de 10 min en fin de bloc de thématiques, voir [modalités](modalites.md). Questions de cours uniquement (définitions, garanties, compromis, nommer la [fallacy](../supports/fallacies.md) menaçante dans une situation décrite en 2 lignes). Aucun code à écrire.

## Séquence finale : code et story
Une seule séquence, individuelle, en classe, sans IA. Elle combine deux types de questions ; l'enseignant choisit (ou l'élève tire) lesquelles poser. Papier **ou** oral : à l'oral, l'élève répond à voix haute devant l'enseignant, sur le même énoncé.

### Question Code
On donne un extrait de code qui contient un défaut lié à une [fallacy](../supports/fallacies.md) (doublon traité deux fois, retry sans backoff, `ts` utilisé pour l'ordre, signature qui ne couvre pas tout…).  
L'élève **explique** (nomme le défaut, la fallacy, le scénario de panne) puis **corrige** (quelques lignes ou en prose, pas de compilation).

### Question Story
On donne une user story. L'élève rédige le **prompt** qu'il donnerait à une IA (ou décrit sa conception, à l'oral). La réponse est notée :

| Critère               | Attendu                                                                                                                                       |
|-----------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| Concepts              | nomme les notions de théorie utiles (QoS, idempotence, LWT, signature…)                                                                       |
| Fallacy               | nomme la fallacy menaçante et ce qui l'illustre                                                                                               |
| Panne                 | précise le comportement en cas d'échec (timeout, doublon, nœud mort)                                                                          |
| Critère d'acceptation | décrit un test vérifiable, conforme au [contrat](../activites/contrats/README.md), reproductible avec le [labo](../activites/infra/README.md) |

## Barème indicatif
Checkpoints : points par question, correction automatique (Forms).  
Séquence finale, chaque question sur 4 : compréhension (2) · justesse (1) · communication (1). Une réponse sans explication vaut 0 sur la compréhension, même si elle est juste.

## Rattachement aux compétences
Chaque tâche est étiquetée avec une ou deux bandes de la [matrice](matrice%20des%20compétences.md) (A2, C1, D1/D2, E1, F1) et un niveau B/I/A.

## Format
- **Checkpoints** : théorie seule, voir ci-dessus.
- **Séquence finale** : prévue au [planning](../planning/README.md).
- **NOTES.md** du [chat](../activites/chat/README.md) : peut servir de point de départ à l'oral (« ce que j'ai compris / ce qui a cassé »).

## Banque d'énoncés

À préparer par l'enseignant, pour la séquence finale. Une ligne par fallacy ; chaque idée existe en question *story* (user story) et en question *code* (défaut à trouver).

| Fallacy                     | Story : user story                                                       | Code : défaut planté                                                          |
|-----------------------------|----------------------------------------------------------------------------------|-------------------------------------------------------------------------------|
| 1 fiabilité                 | « Un transfert doit être exécuté exactement une fois, même si la banque plante » | doublon traité deux fois (pas de déduplication), retry avec un nouvel `id`    |
| 2 latence                   | « Le client doit afficher l'état même si le service est lent »                   | appel sans timeout qui bloque l'interface                                     |
| 3 bande passante            | « La vitrine ne doit pas saturer le réseau »                                     | image non réduite, republiée toutes les secondes                              |
| 4 réseau sûr                | « Seul le titulaire peut consulter son solde »                                   | `from` pris pour argent comptant, signature qui ne couvre pas tous les champs |
| 5 topologie                 | « Un nouveau service doit être découvert sans reconfigurer les autres »          | adresse du service écrite en dur                                              |
| 6 plusieurs administrateurs | « Une équipe change son format de message sans casser les autres »               | récepteur strict qui rejette un champ inconnu                                 |
| 7 coût                      | « Le capteur doit tenir 3 mois sur batterie »                                    | publication toutes les secondes sans nécessité                                |
| 8 homogène                  | « Un client Python doit lire les messages d'un service Java »                    | sérialisation dépendante du langage ou de la machine                          |
| heure                       | « Les messages doivent s'afficher dans l'ordre de la conversation »              | tri par `ts` local sans correction                                            |

## Annexes par fil rouge

La grille est la même pour tous les fils rouges. Chaque fil rouge ajoute une annexe : le terrain utilisé pour la séquence finale et les adaptations de la banque d'énoncés.

- [UMDD](annexes/umdd.md)
- [Powercher](annexes/powercher.md)
- [Pedaloland](annexes/pedaloland.md)
- [Gabarit pour un nouveau fil rouge (dont le libre)](annexes/modele.md)
