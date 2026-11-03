# C1 — Hello chat

|                 |                                                                                   |
|-----------------|-----------------------------------------------------------------------------------|
| Thématique | [Sérialisation](../../thematiques/01-serialisation.md) |
| Drill préalable | [D01](../drills/D01-serialisation.md)                                             |
| Fallacy         | 3 (bande passante), 8 (réseau homogène)                                           |
| Théorie         | [Sérialisation](../../supports/Sérialisation.md) · [MQTT](../../supports/mqtt.md) |

## But
Un programme (sans interface graphique obligatoire) qui envoie et reçoit des messages `chat` valides.

## Étapes

**0. Choisissez et justifiez votre langage.** Créez `LANGUAGE.md` : bibliothèque MQTT retenue (nom et version), pourquoi, une limite que vous connaissez. Trois lignes.  
→ *Pas de « parce que je connais » seul* : quelle alternative avez-vous écartée ?

**1. Connectez-vous au broker** `localhost:1883` avec comme identifiant client **votre prénom**.  
La **racine des topics** (`chat`) doit être un paramètre de configuration, pas une constante dans le code : vous la changerez à la fin de la spirale.  
→ Vérifiez dans `docker compose logs broker --tail 5` : `New client connected from … as <prénom>`.

**2. Abonnez-vous** à `chat/all` et affichez chaque message reçu (auteur, texte).  
→ Vérifiez : depuis un autre terminal, publiez à la main une enveloppe valide (voir l'[exemple du contrat](../contrats/README.md#enveloppe)) avec `DC mosquitto_pub -t chat/all -m '<enveloppe>'`. Votre programme l'affiche.

**3. Envoyez** : votre programme construit une enveloppe valide (nouvel `id`, `ts` en UTC, `type` `chat`) et la publie sur `chat/all`.  
→ Vérifiez : `DC mosquitto_sub -t 'chat/#' -v` montre un JSON qui **valide** contre [envelope](../contrats/envelope.schema.json) **et** [chat](../contrats/chat.schema.json).

**4. Entrée hostile.** Publiez à la main : `pas du json` ; `{"v":1}` ; une enveloppe valide avec `"type":"inconnu"`.  
→ Votre chat ne plante pas ; il écrit une ligne `message ignoré : <raison>` pour chacun.

**5. Votre écho.** Le broker vous renvoie vos propres messages. Décidez : les afficher à l'envoi ou à la réception, mais **une seule fois**.  
→ Indice : [NoLocal](../../supports/mqtt.md#nolocal) existe en MQTT 5 ; sinon, comparez `from` à votre nom.

**6. Mesurez.** Après chaque envoi, affichez la taille du message en octets.  
→ Comparez avec D01 : 144 octets pour l'exemple du contrat. Où passent les octets ?

## Terminé quand

- [ ] `LANGUAGE.md` écrit
- [ ] un message de votre programme est affiché chez un camarade (autre langage si possible)
- [ ] 3 entrées hostiles ignorées proprement
- [ ] vous savez dire pourquoi `ts` est en UTC et d'où vient l'`id`

## Si vous utilisez une IA
Votre prompt doit citer : le contrat (lien/fichier), le schéma, le comportement face à un message invalide, et un test d'acceptation (étape 4). Voir la [grille](../../evaluation/grille.md).
