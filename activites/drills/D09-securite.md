# D09 — Sécurité : signature, chiffrement, rejeu

| | |
|---|---|
| Thématique | [Sécurité](../../thematiques/09-securite.md) |
| Fallacy | 4 (le réseau est sûr) |
| Durée | 25 min |
| Matériel | papier ; un terminal avec `openssl` (Git Bash, WSL ou Linux) |
| Théorie | [Sécurité](../../supports/securite.md) · [Confidentialité](../../supports/confidentialite.md) · [Réseau de confiance](../../supports/reseau-confiance.md) |

## But
Associer chaque attaque à la propriété violée et au mécanisme qui la protège, puis éprouver une signature à la main.

## 1. Quatre messages interceptés (papier)

Sur le broker ouvert de la classe, un attaquant observe `lab/bank`. Pour chaque cas, nommez la propriété violée parmi : **confidentialité**, **intégrité**, **authenticité** (qui est l'auteur), **fraîcheur** (le message est-il récent ?) ; puis le mécanisme qui protège.

| # | Ce qui s'est passé | Propriété violée | Mécanisme |
|---|---|---|---|
| 1 | Bob lit en clair le solde d'Alice | ? | ? |
| 2 | Le montant passe de 10 à 1000 pendant le transit | ? | ? |
| 3 | Mallory publie un transfert avec `"from":"alice"` | ? | ? |
| 4 | Mallory renvoie telle quelle, hier, une vraie demande de transfert d'Alice | ? | ? |

→ 1 confidentialité · chiffrement · 2 intégrité · signature/MAC · 3 authenticité · signature (la clé prouve l'auteur) · 4 **fraîcheur** · identifiant unique et horodatage **à l'intérieur** de ce qui est signé.

## 2. Signer à la main (openssl)

Alice et la banque partagent le secret `s3cret` (clé symétrique). Un MAC (code d'authentification de message) prouve que le message vient de quelqu'un qui connaît le secret et n'a pas été modifié.

5. Calculez le MAC du message :  
   `echo -n '{"text":"salut"}' | openssl dgst -sha256 -hmac "s3cret"`  
   → `867cd6d4…45ff` (64 caractères hexadécimaux).
6. Changez **une lettre** (`salur`) et recalculez.  
   → `8c6f0562…e76b` : totalement différent. Toute modification est détectée.
7. Sans connaître `s3cret`, pouvez-vous produire le MAC d'un message modifié ? Essayez avec un secret faux (`autre`).  
   → le MAC ne correspond pas.

## 3. Le rejeu

8. Un transfert signé : `{"id":"t42","amount":10}` avec son MAC. Mallory le copie et le republie dix fois.  
   Le MAC est-il toujours valide ? La banque exécute-t-elle dix fois le transfert ?  
   → MAC **valide** (le message est identique) ; sans protection, **dix transferts**.
9. Qu'ajoutez-vous **dans le message signé** pour que les neuf répétitions soient refusées ?  
   → `id` unique déjà vu (D05) et `ts` récent : un message vieux de plus de, par exemple, 30 s est refusé. (Retour à D02 : cela demande des horloges raisonnablement synchrones.)

## 4. Symétrique ou asymétrique ?

Complétez en vous aidant de la théorie :

| | Clé partagée (symétrique) | Paire de clés (asymétrique) |
|---|---|---|
| Qui connaît le secret ? | ? | ? |
| Peut-on prouver l'auteur parmi 30 nœuds ? | ? | ? |
| Combien de clés pour 30 participants ? | ? | ? |

→ tous ceux qui partagent · non (n'importe qui du groupe a pu signer) · 30 paires, mais il faut savoir à qui appartient chaque clé publique (réseau de confiance, certificats).

## Débrief

- Le chiffrement seul garantit-il que le message n'a pas été modifié ? La signature seule garantit-elle qu'il n'a pas été lu ?
- Qui vérifie qu'une clé publique est bien celle d'Alice ? Que se passe-t-il si c'est la classe qui le décide ?
- Quel élément de l'[enveloppe](../contrats/README.md) servira à la fraîcheur, lequel à l'identité ?

<details><summary>Solution</summary>

Non / non : les deux sont indépendants (confidentialité ≠ authenticité). L'identité d'une clé publique repose sur un tiers de confiance (autorité, ou accord de la classe). Fraîcheur : `id` + `ts` ; identité : `from` (à prouver par la signature).
</details>
