# D06 — Transaction : Fair Trade quand la banque plante

| | |
|---|---|
| Thématique | [Transactions](../../thematiques/06-transactions.md) |
| Fallacy | 1 (fiabilité), 6 (plusieurs administrateurs) |
| Durée | 25 min |
| Matériel | papier, une feuille par groupe de 2 |
| Théorie | [Transaction](../../supports/transaction.md) : *Transactions compensatoires (Sagas)* et *Rôle de l'Idempotence* |

## But
Enumérer les états intermédiaires d'une opération qui touche deux services, et en déduire un ordre d'opérations et une compensation.

## Situation
Alice dépose **10 croissants** à la **Coopérative** (service A). La Coopérative doit lui verser **20 CHF** via la **Banque** (service B). Deux services, deux équipes, un réseau entre les deux. Deux opérations :

- **S** : *stock* : ajouter 10 croissants au stock de la Coop.
- **P** : *paiement* : la Banque transfère 20 CHF de la Coop à Alice.

## Étapes

1. **Ordre S puis P.** Dessinez la ligne du temps avec un crash possible entre S et P. Remplissez l'état final dans ce cas :

   | | Stock Coop | Compte Alice | Cohérent ? |
   |---|---|---|---|
   | crash entre S et P | ? | ? | ? |

   → stock +10, Alice **non payée** : incohérent (Alice a perdu ses croissants).

2. **Ordre P puis S.** Même tableau, crash entre P et S.  
   → Alice payée, stock **inchangé** : incohérent (la Coop a payé des croissants qu'elle n'a pas).

3. **Le cas le plus vicieux.** Ordre S puis P. La Coop envoie l'ordre de paiement. La Banque l'exécute puis **plante avant de répondre**. Du point de vue de la Coop, que s'est-il passé ? Cochez ce qu'elle *sait* :  
   ☐ le paiement a eu lieu ☐ le paiement n'a pas eu lieu ☐ elle ne sait pas  
   → **elle ne sait pas** : sans réponse, les deux sont possibles. C'est la fallacy 1.

4. **Que faire de ce doute ?** Trois options : renvoyer l'ordre ; ne rien faire ; annuler S. Pour chacune, dites ce qui arrive si le paiement avait *vraiment* eu lieu, et s'il n'avait *pas* eu lieu.

   | Option | Paiement déjà fait | Paiement pas fait |
   |---|---|---|
   | renvoyer | ? | ? |
   | ne rien faire | ? | ? |
   | annuler S | ? | ? |

   → renvoyer : *double paiement* si la banque n'est pas idempotente, correct sinon ; ne rien faire : correct / Alice non payée ; annuler S : Alice payée **et** stock repris.

5. **Proposez un protocole.** Utilisez : un identifiant de transaction `t1`, l'idempotence de la Banque (D05), un état de la transaction (`en cours`, `payée`, `annulée`). Dessinez le diagramme de séquence **nominal**, puis le cas « la banque plante avant de répondre ».

6. **Compensation.** Si la Banque répond « fonds insuffisants » (un échange entre deux services, par exemple), quelle action rétablit la cohérence ? Dans quel ordre ?

## Débrief

- Qu'est-ce qui rend impossible une « vraie » transaction ACID sur ces deux services ? (pensez aux deux administrateurs, fallacy 6)
- Quel est le coût d'une saga par rapport à une transaction ? (états visibles pendant l'opération)

<details><summary>Solution</summary>

Protocole minimal : la Coop enregistre `t1: en cours` + stock réservé ; envoie `transfer(id=t1)` ; sans réponse, **renvoie le même** `transfer(id=t1)` jusqu'à obtenir une réponse (la Banque idempotente rend l'opération sûre) ; sur `ok` → `t1: payée`, stock confirmé ; sur `refus` → `t1: annulée`, **compensation** : restituer les croissants à Alice. Pendant la transaction, l'état intermédiaire (stock réservé) est visible : on obtient une cohérence *à terme*, pas instantanée.
</details>
