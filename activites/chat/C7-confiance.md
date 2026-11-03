# C7 — Prouver l'auteur, refuser le rejeu

| | |
|---|---|
| Thématique | [Sécurité](../../thematiques/09-securite.md) |
| Drill préalable | [D09](../drills/D09-securite.md) |
| Fallacy | 4 (réseau sûr), 6 (plusieurs administrateurs) |
| Théorie | [Sécurité](../../supports/securite.md) · [Confidentialité](../../supports/confidentialite.md) · [Réseau de confiance](../../supports/reseau-confiance.md) |

## But
Les messages `chat` sont signés ; un message modifié, usurpé ou rejoué est refusé.

## Étapes

**1. Amendez le contrat (en classe).** Aujourd'hui l'[enveloppe](../contrats/envelope.schema.json) n'a pas de signature. Proposez, par pull request sur `activites/contrats`, une version `v: 2` avec un champ optionnel `sig`. La classe doit trancher **trois questions** avant que quiconque code :

| Question | Pourquoi c'est difficile |
|---|---|
| Que signe-t-on exactement ? | Deux langages sérialisent le même JSON avec des espaces ou un ordre de clés différents : la signature ne se vérifie pas. |
| Quelle clé, partagée avec qui ? | Un secret par paire ? par groupe ? une paire de clés par nœud ? |
| Que fait un récepteur v1 d'un message v2 ? | Si l'enveloppe est stricte, il le rejette (fallacy 6). |

→ Indice : cherchez « JSON canonicalization » ; ou signez une chaîne que vous construisez vous-mêmes à partir des champs, dans un ordre fixé par le contrat.

**2. Signez** chaque message sortant.  
**3. Vérifiez** chaque message entrant ; refusez et journalisez : `signature invalide`, `signature absente`.  
→ Vérifiez : prenez un message signé et changez **une lettre** du `text` à la main avant de le republier avec `mosquitto_pub` : refusé.

**4. Usurpation.** Publiez à la main un message avec `"from":"alice"` et une signature calculée avec **votre** clé.  
→ Refusé : la clé ne correspond pas à l'auteur annoncé.

**5. Rejeu.** Republiez à l'identique un message valide, deux fois puis après 2 minutes.  
→ Le deuxième envoi est refusé par la déduplication (C5) ; l'envoi tardif par une règle de fraîcheur sur `ts` (par exemple ±30 s). Notez la règle.

**6. Stockage de la clé.** Où votre programme lit-il la clé ? (Pas dans le dépôt Git.)

**7. (Facultatif) Confidentialité.** Chiffrez le `text` des messages privés. Qui connaît la clé ? Que voit l'observateur sur `chat/dm/#` ?

## Terminé quand

- [ ] les trois attaques (modification, usurpation, rejeu) sont refusées
- [ ] la décision de signature est dans le contrat (pas seulement dans votre tête)
- [ ] aucun secret dans le dépôt
- [ ] vous pouvez dire ce que votre signature **ne** protège **pas** (la lecture, par exemple)

## Si vous utilisez une IA
Prompt : nommer MAC/HMAC ou signature asymétrique, **ce qui est signé et dans quel ordre**, la règle de fraîcheur, et les trois tests d'attaque. Un prompt qui ne dit pas ce qui est signé produit du code non interopérable.
