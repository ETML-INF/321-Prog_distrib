# Idempotence

![idempotence.png](../supports/assets/idempotence.png)

Un message peut arriver deux fois, ou pas du tout. Comment faire pour que l'effet ne se produise qu'une fois ?

**À l'issue de cette thématique, vous serez capable de :**
- classer une opération (idempotente ou non)
- dédupliquer par identifiant et borner la mémoire nécessaire
- expliquer pourquoi dédupliquer ne suffit pas quand l'ordre compte
- décrire l'idempotence d'une API HTTP

**Fallacies :**

- [1 — Le réseau est fiable](../supports/fallacies.md#fallacy-1)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [Idempotence](../supports/idempotence.md)
- [Kahoot](https://create.kahoot.it/details/af92797e-3495-40b3-8a37-ae8d9d5bfbf9)
- Slides : [Idempotence](../slides/idempotence/)

## Drill
- [D05 — Idempotence : le solde après les doublons](../activites/drills/D05-idempotence.md)

## Chat
- [C5 — Ignorer les doublons](../activites/chat/C5-doublons.md)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Paiement](../activites/fil-rouge/services.md#cat-paiement)
- **Doit introduire :** une opération à effet (montant, stock, compteur) qui peut être rejouée
- **Preuve attendue :** un message reçu deux fois n'altère pas le résultat ; la mémoire de déduplication est bornée
- **Décision à documenter :** la durée ou la taille de la mémoire de déduplication, et ce qui se passe quand elle déborde

## Fil rouge

| Fil rouge  | Activité                                                                         |
|------------|----------------------------------------------------------------------------------|
| UMDD       | [Banque, coopérative, Fair Trade](../activites/fil-rouge/umdd/banque-coop.md)    |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#idempotence-transactions) |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                               |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |

Activité alternative : [Idempotence en pratique](../activites/idempotence/README.md)
