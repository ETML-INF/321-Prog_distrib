# Transactions

Quand une opération touche deux services qui ne se font pas confiance, comment rester cohérent ?

**À l'issue de cette thématique, vous serez capable de :**
- décrire les états intermédiaires d'une opération qui touche deux services
- distinguer transaction distribuée et saga
- concevoir une compensation
- expliquer le rôle de l'idempotence dans une transaction

**Fallacies :**

- [1 — Le réseau est fiable](../supports/fallacies.md#fallacy-1)
- [6 — Il n’y a qu’un administrateur](../supports/fallacies.md#fallacy-6)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [Transaction](../supports/transaction.md)
- Slides : [Transaction](../slides/transaction/){target="_self"}

## Drill
- [D06 — Transaction : Fair Trade quand la banque plante](../activites/drills/D06-transaction.md)

## Chat
- (pas d'étape de chat dédiée : la déduplication de [C5](../activites/chat/C5-doublons.md) est le prérequis)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Échange](../activites/fil-rouge/services.md#cat-echange)
- **Doit introduire :** une opération à au moins deux participants qui doit réussir en entier ou pas du tout
- **Preuve attendue :** une panne provoquée au milieu de l'opération laisse un état cohérent (rien de perdu, rien de créé)
- **Décision à documenter :** compensation ou abandon en cas d'échec, et qui coordonne

## Fil rouge

| Fil rouge  | Activité                                                                         |
|------------|----------------------------------------------------------------------------------|
| UMDD       | [Banque, coopérative, Fair Trade](../activites/fil-rouge/umdd/banque-coop.md)    |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#idempotence-transactions) |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                               |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |

Activité alternative : [Transat : transaction distribuée](../activites/transat/README.md)
