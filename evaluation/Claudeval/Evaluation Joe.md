# Évaluation Joe

|                      |                                                                |
| -------------------- | -------------------------------------------------------------- |
| Candidat             | Joe                                                            |
| Module               | 321 – Systèmes distribués (fil rouge UMDD)                     |
| Référentiel          | matrice des compétences.md                                     |
| Début de l'entretien | 2026-10-05 13:35                                               |
| Fin de l'entretien   | 2026-10-05 14:45                                               |
| Format               | 24 questions (3 par bande), relances limitées à 2 par question |

## Synthèse

| Bande                                        | Niveau atteint   | Commentaire court                                                                            |
| -------------------------------------------- | ---------------- | -------------------------------------------------------------------------------------------- |
| A1 – Analyser et transférer des logiciels    | **B** (fragile)  | Décrit l'application de départ ; analyse de l'adéquation au distribué très limitée           |
| A2 – Architectures distribuées / intégration | **B**            | Notions de base présentes ; stratégie d'intégration sans gestion des échecs                  |
| B1 – Gestion des données                     | **B**            | Exigences de base comprises ; déploiement (Docker) non maîtrisé                              |
| C1 – Composants                              | **B**, I partiel | Rôles des composants corrects ; implémentation et tests peu rigoureux                        |
| D1 – Échange de données (principes)          | **I**, A partiel | Bonne compréhension du pub/sub ; idée pertinente d'ID de transaction                         |
| D2 – Protocoles d'interface                  | **B**, I partiel | Connaît peu de protocoles ; structure de topics correcte mais confidentialité mal traitée    |
| E1 – Authentification / autorisation         | **I** partiel    | Distinction et usage des clés corrects ; contenu de la signature, rejeu et TLS mal compris   |
| F1 – Surveillance                            | **B**, I partiel | Polling et LWT connus ; idée intéressante de transaction synthétique, mise en œuvre sommaire |

**Profil général** : Joe a une compréhension correcte des concepts fondamentaux du fil rouge : pub/sub, sérialisation, nœuds, clés publiques et privées. Il ou elle mobilise surtout ce qui a été pratiqué en classe. Les réponses restent souvent courtes et nécessitent des relances. Les aspects robustesse (échecs, concurrence, rejeu, confidentialité réelle) et déploiement (Docker, persistance) sont les principales lacunes. Le niveau global se situe entre **Débutant consolidé** et **Intermédiaire en cours d'acquisition**.

---

## Détail par bande

### A1 – Analyser et transférer des logiciels → **B (fragile)**

- **A1B** : Joe décrit l'application WinForms (classes du modèle, formulaires d'affichage, aucune communication). Il ou elle qualifie l'architecture de « MVC » : c'est un patron de conception interne, pas la nature de l'application (monolithique, autonome). Joe identifie que les données viendront des services (Coopérative, Banque) et que MQTT sera nécessaire. → *Partiellement atteint.*
- **A1I** : Un seul problème identifié (« pas de moyen de communication ») ; Joe ne sait pas citer d'autre obstacle (données codées en dur, sérialisation, réception asynchrone et thread UI). → *Non atteint.*
- **A1A** : Joe décrit une migration plausible vers l'Annuaire : `iamopen` avec le `Business` sérialisé, désérialisation de la liste, affichage dans le `Billboard`. Erreurs et oublis : topic `/repertory` au lieu de `/directory`, `iamout` au lieu de `iamclosed`. Joe ne mentionne ni le LWT, ni la mise à jour de l'UI depuis un thread non-UI, ni le remplacement de la liste (le code ajoute sans vider, d'où un risque de doublons). → *Non atteint.*

### A2 – Analyser et transférer des logiciels (architectures) → **B**

- **A2B** : Joe définit les microservices (plusieurs programmes, chacun fait une chose). Avantages : modification indépendante, redondance. Inconvénient : plus de code. Après relance, Joe ne trouve pas de second inconvénient (latence réseau, cohérence des données, complexité de déploiement et de débogage). → *Atteint de justesse.*
- **A2I** : Joe cite des conventions (topics), un fichier de configuration (adresse du broker) et la sérialisation. Il ou elle ne mentionne pas le comportement au démarrage et à l'arrêt. → *Partiellement atteint.*
- **A2A** (Fair Trade) : La séquence nominale est correcte (dépôt → calcul du prix → transfert → enregistrement). En cas d'échec : la Banque prévient la Coopérative, qui prévient la BP. Joe ne traite pas le sort des produits (restitution), ni l'ordre des opérations (enregistrer avant ou après paiement), ni le cas où la Banque ne répond pas. → *Non atteint.*

### B1 – Comprendre et planifier la gestion des données → **B**

- **B1B** : Joe cite la persistance et la confidentialité des données. Il ou elle propose un stockage hors du conteneur, avec redondance. Sur la concurrence, Joe répond que la Coopérative unique sérialise les demandes (« la deuxième fera erreur ») : l'intuition est juste mais pas argumentée. → *Atteint.*
- **B1I** : Joe ne sait pas déployer un stockage persistant avec Docker (volume, `docker-compose`, base de données en conteneur). → *Non atteint.*
- **B1A** : Joe choisit l'Annuaire en mémoire (justifié : l'état se reconstruit via `whoisopen`) et MySQL pour la Banque (accès concurrents). La justification reste partielle : pas de mention des transactions ACID ni de l'intégrité des soldes. → *Partiellement atteint, mais sans le niveau I.*

### C1 – Expliquer, mettre en œuvre et évaluer les composants → **B**, I partiel

- **C1B** : Rôles corrects : BP et services sont des nœuds, le broker sert à la communication, Docker à l'exécution des services sur le serveur. La compréhension de ce qu'apporte Docker reste superficielle (« lancer facile »). → *Atteint.*
- **C1I** : Le code du `logger` est approximatif et ne respecte pas l'usage spécifié. Le format `--source=<name>` n'est pas géré, `--broker` n'est pas traité, et sans argument le programme affiche l'aide au lieu d'utiliser les valeurs par défaut. Les tests sont manuels et sommaires (test local avec un conteneur et `--broker`, puis test avec Julien via `--source`). Les points de la DoD (« died! », log de toutes les BP) n'ont pas été évoqués. → *Partiellement atteint.*
- **C1A** : Critères d'évaluation des annuaires : démarrage simple, absence de bugs, confiance en l'auteur. Les tests proposés reprennent la DoD (apparition et disparition des BP). Joe n'évoque ni la robustesse face aux morts brutales, ni l'idempotence, ni la lisibilité ou la maintenabilité du code. → *Non atteint.*

### D1 – Expliquer et mettre en œuvre l'échange de données → **I**, A partiel

- **D1B** : Bonne explication du pub/sub : l'émetteur ignore les destinataires et l'absence d'accusé de réception, les abonnés reçoivent via le broker. Comparaison pertinente avec HTTP (réponse explicite). → *Atteint.*
- **D1I** : Le code de sérialisation et désérialisation JSON est correct sur le principe. Joe sait que les images sont encodées en base64 et a vérifié l'objet avant et après au débogueur. → *Atteint.*
- **D1A** : Joe propose un mécanisme requête/réponse sur MQTT (topic `/money`), suivi d'une demande de solde en cas de succès et d'un log en cas d'absence de réponse. Après relance, il ou elle propose un **identifiant de transaction** pour corréler les réponses. Joe ne précise ni le topic de réponse, ni le délai d'attente. → *Partiellement atteint.*

### D2 – Expliquer et mettre en œuvre l'échange de données (protocoles) → **B**, I partiel

- **D2B** : Joe cite MQTT (bus de messages), HTTP (client/serveur) et HTTPS (chiffré). HTTPS n'est pas un type de protocole distinct de HTTP. Joe ne connaît pas d'autres familles (WebSocket, gRPC/RPC, AMQP…). → *Atteint de justesse.*
- **D2I** : Structure `/chat/<destinataire>` et code d'envoi corrects. L'expéditeur est identifié par le `senderId` de l'enveloppe. Erreur technique : publier sur `/chat/#` pour un message global n'est pas possible, les jokers étant réservés aux abonnements. Joe reconnaît correctement que n'importe qui peut s'abonner à `/chat/#` et lire les messages personnels. → *Partiellement atteint.*
- **D2A** : Pas de réponse spontanée. Après relance : MQTT adapté au logging (diffusion, consultable partout), HTTP préférable pour une application mobile publique. Le cas de la Banque n'est pas traité. → *Partiellement atteint.*

### E1 – Mise en œuvre de l'authentification et de l'autorisation → **I** partiel

- **E1B** : Distinction claire (« t'es qui ? » / « t'as pas le droit »). Exemples pertinents : signature du virement avec la clé privée ; refus d'un virement depuis le compte de la Coopérative. → *Atteint.*
- **E1I** : Schéma correct : clé privée côté BP, clés publiques de tous côté Banque, recherche de la clé publique par émetteur, validation. Le contenu de `sign()` et `validate()` reste flou (« du code de chiffrement RSA ») : Joe ne précise pas que la signature porte sur le message sérialisé ou son empreinte. Il ou elle ne connaît pas la protection contre le rejeu (nonce, horodatage, ID de transaction pourtant évoqué en D1). Test proposé : « bidouiller la clé », sans démarche structurée. → *Partiellement atteint.*
- **E1A** : Sens du chiffrement correct (clé publique de la Banque pour chiffrer, clé privée pour déchiffrer). **Conception erronée** : Joe pense que TLS sur le broker suffit à empêcher les autres BP de lire les opérations bancaires. Après relance, Joe admet qu'une autre BP peut s'abonner, sans en tirer la conséquence. TLS protège le transport entre le client et le broker, pas le contenu vis-à-vis des autres abonnés. → *Non atteint.*

### F1 – Utilisation des outils de surveillance → **B**, I partiel

- **F1B** : Joe définit la surveillance comme une application qui « pingue » les services, et cite le temps de réponse comme autre métrique. Il ou elle cite un outil, « powerwatch », non identifié. Après relance, Joe identifie le LWT comme mécanisme de détection utilisé dans UMDD. → *Atteint de justesse.*
- **F1I** : Boucle de polling avec envoi de `ping` sur plusieurs topics, réception dans `OnMessageReceived`, seuil de panne à 5 secondes. Manques : pas de pause entre les itérations (boucle infinie qui sature le broker), pas de suivi par service de la dernière réponse, topics des services approximatifs. → *Partiellement atteint.*
- **F1A** : Idée pertinente de **transaction synthétique** : un compte de test crédité toutes les 10 secondes, avec vérification de l'augmentation du solde. Alerte par email. La configuration reste minimale : timeout et une adresse d'alerte, sans définition par service, sans seuils de gravité, sans délai avant alerte ni gestion des alertes répétées. → *Partiellement atteint.*

---

## Points forts

- Bonne compréhension du modèle publish/subscribe et de ses limites (absence d'accusé de réception).
- Sérialisation JSON maîtrisée, y compris la gestion des images en base64.
- Rôles respectifs des clés publiques et privées corrects pour la signature et le chiffrement.
- Quelques idées pertinentes : ID de transaction pour corréler les réponses, transaction synthétique pour superviser la Banque.

## Axes de progression prioritaires

1. **Gestion des échecs dans les échanges distribués** : timeouts, réponses manquantes, compensation (restitution des produits dans Fair Trade), idempotence.
2. **Déploiement avec Docker** : volumes pour la persistance, `docker-compose`, configuration par variables d'environnement.
3. **Sécurité** : contenu exact d'une signature, protection contre le rejeu, différence entre sécurité du transport (TLS) et chiffrement de bout en bout.
4. **Rigueur d'implémentation** : respect des spécifications (arguments du `logger`, noms de messages et de topics), règles MQTT (pas de joker en publication).
5. **Tests** : passer de vérifications manuelles informelles à des scénarios de test explicites, alignés sur la DoD.
6. **Culture des protocoles** : connaître d'autres familles de protocoles (WebSocket, RPC/gRPC, AMQP) et savoir argumenter un choix selon le cas d'usage.

# Transcription de l'entretien d'évaluation – Joe

- **Date** : 5 octobre 2026
- **Début** : 13:35 – **Fin** : 14:45
- **Module** : 321 – Développement de systèmes distribués (fil rouge « UMDD »)
- **Participants** : Joe, apprenant, et Claude (IA), examinateur
- **Référentiel** : `matrice des compétences.md`
- **Protocole** : `Entretien.md`
- **Rapport issu de l'entretien** : `Evaluation Joe.md`

> Les messages de l'apprenant sont reproduits tels quels (fautes de frappe comprises).
>
> Un premier départ, à 13:33, a été interrompu par l'apprenant après la première question pour modifier le protocole (ajout de la règle « trois questions par bande »). L'entretien a été relancé intégralement à 13:35 ; seule cette seconde session est transcrite ci-dessous.

---

## Protocole (Entretien.md)

> J'ai étudié le développement de systèmes distribués en m'appuyant sur les contenus de ce dossier 321-MASTER.
>
> Le plan de travail du module est décrit dans "nouveau fil rouge.md"
>
> La matrice des compétences visées par le cours est "matrice de compétences.md"
>
> Interroge-moi pour évaluer mon niveau de compétence par rapport à cette matrice.
>
> Tu ne poses qu'une question à la fois.
> Tu me poses trios questions par bande.
> Tu peux me demander du code C#, mais sans être strict sur la syntaxe.
> Tu ne donnes aucun feedback après mes réponses, mais tu peux me relancer (maximum deux fois) si ma réponse est imprécise
>
> Au terme de l'entretien, tu dois générer un rapport "Evaluation (mon nom).md"
> Les dates début/fin doivent figurer dans le rapport

## Introduction (Claude)

Bonjour Joe. L'entretien recommence (début : 2026-10-05 13:35).

Il couvre les 8 bandes de la matrice, à raison de trois questions par bande (24 questions au total). Je vous poserai une question à la fois.

---

## Bande A1 – Analyser et transférer des logiciels

### Question A1-1

**Claude :**
Dans la base de code fournie au départ, la vitrine (`Store`) et le `Billboard` sont des formulaires Windows Forms. Les objets `Business` et `Specialty` y étaient d'abord codés en dur.

Comment décririez-vous la structure de cette application de départ ? Dites-moi quel type d'architecture c'est, comment ses parties sont organisées et où se trouvent les données. Expliquez aussi en quoi elle diffère d'une application organisée en microservices comme UMDD.

**Joe :**
> c'est une application winforms. il y a des classes pour le modèle. il y a les formulaires pour afficher.
> elle communique avec rien

**Claude (relance 1) :**
Vous dites que l'application ne communique avec rien. Comment appelle-t-on ce type d'architecture ?

Et concrètement, qu'est-ce qui change dans la structure de l'application quand on passe à UMDD, où les données (BP, spécialités, comptes) sont réparties entre plusieurs services ?

**Joe :**
> c'est MVC
> les données sont pas dans le programme, on va les prendre à la coopérative ou la banque

**Claude (relance 2) :**
Si les données ne sont plus dans le programme, quels éléments faut-il ajouter ou modifier dans l'application pour qu'elle fonctionne avec UMDD ?

**Joe :**
> mqtt

### Question A1-2

**Claude :**
Prenons la vitrine `Store` de votre BP telle qu'elle était au départ : un objet `Business` codé en dur, affiché directement dans le formulaire.

Qu'est-ce qui, dans cette application, rendait difficile son intégration dans un système distribué ? Citez au moins deux problèmes concrets.

**Joe :**
> pas de moyen de communication

**Claude (relance 1) :**
Vous avez cité un problème. Quel serait un deuxième problème ?

Pensez par exemple à ce qui se passe quand une autre BP veut obtenir votre `Business`, ou quand un message arrive de manière asynchrone pendant que le formulaire est affiché.

**Joe :**
> je sais pas

### Question A1-3

**Claude :**
Vous devez migrer votre BP pour qu'elle s'annonce à l'Annuaire au démarrage et qu'elle affiche dans le `Billboard` la liste des BP ouvertes envoyée par l'Annuaire.

Décrivez les modifications à apporter à votre application. Précisez les classes ou méthodes à ajouter ou modifier, ce qui se passe au démarrage, à la réception d'un message et à la fermeture. Vous pouvez écrire du C# approximatif.

**Joe :**
> envoyer 'iamopen' sur /repertory dans le constructeur de ma BP.
> lire le message qui a la liste des BP ouvertes , enrigistrer et afficher dans bilboard

**Claude (relance 1) :**
Concrètement, que contient le message `iamopen` que vous envoyez, et sous quelle forme ?

Et que doit faire votre BP lorsqu'elle se ferme ?

**Joe :**
> il y a mon objet business sérialisé.
> envoyer iamout

**Claude (relance 2) :**
Quand le message contenant la liste des BP ouvertes arrive, il est reçu dans la méthode appelée de manière asynchrone par l'agent MQTT.

Pouvez-vous écrire, même approximativement, le code de cette méthode jusqu'à l'affichage dans le `Billboard` ?

**Joe :**
```
List<Business> message = envelope.Msg.Deserialize()
foreach (Business b in message) Billboard.Add(business)
foreach (Business b in BillBoard.Businesses) b.display()
```

---

## Bande A2 – Analyser et transférer des logiciels (architectures)

### Question A2-1

**Claude :**
Qu'est-ce qu'une architecture microservices ? Citez deux avantages et deux inconvénients par rapport à une application monolithique, en vous appuyant si possible sur UMDD.

**Joe :**
> plusieurs programmes (noeud). chacun fait un truc précis.
> on peut modifier un programme sans changer un autre.
> ça fait beaucoup de code

**Claude (relance 1) :**
Il manque encore un avantage et un inconvénient. Lesquels citeriez-vous, en pensant par exemple à ce qui se passe quand un service tombe en panne, ou à la façon dont les services échangent leurs données ?

**Joe :**
> si un répond pas, un autre peut répondre

**Claude (relance 2) :**
Et un deuxième inconvénient ?

**Joe :**
> jsp

### Question A2-2

**Claude :**
Vous voulez qu'un programme existant puisse échanger avec les autres services d'UMDD. C'est le cas de votre client `logger` en ligne de commande ou de votre BP.

Que faut-il mettre en place ou adapter dans ce programme pour le préparer à cette intégration ? Pensez au format des données échangées, aux paramètres de connexion et au comportement au démarrage et à l'arrêt.

**Joe :**
> des conventions. par exemple les topics.
> un fichier de configuration

**Claude (relance 1) :**
Pouvez-vous préciser ? Quelles conventions faut-il établir en plus des topics ? Et que mettriez-vous concrètement dans ce fichier de configuration ?

**Joe :**
> l'adresse du broker

**Claude (relance 2) :**
Et pour le format des données échangées, et le comportement de la BP au démarrage et à l'arrêt, que faut-il prévoir ?

**Joe :**
> sérialiser

### Question A2-3

**Claude :**
Dans la thématique « Fair Trade », quand une BP dépose une production à la Coopérative, celle-ci doit créditer le compte de la BP à la Banque. Si le compte de la Coopérative n'est pas suffisamment approvisionné, la BP doit récupérer ses produits.

Comment organiseriez-vous les échanges entre la BP, la Coopérative et la Banque pour réaliser cette intégration ? Décrivez la séquence des messages et ce qui se passe en cas d'échec.

**Joe :**
> - BP envoi des produits
> - Coop calcule le prix
> - coop fait un transfert à la banque
> - coop enregistre les produits

**Claude (relance 1) :**
Et en cas d'échec, par exemple si le solde de la Coopérative est insuffisant ou si la Banque ne répond pas, que se passe-t-il ? Comment la Coopérative et la BP en sont-elles informées ?
v
**Joe :**
> un message d'erreur

**Claude (relance 2) :**
Qui envoie ce message d'erreur, à qui, et que deviennent les produits que la BP a envoyés ?

**Joe :**
> la banque envoie à la coop
> la coop envoie un message à BP

---

## Bande B1 – Comprendre et planifier la gestion des données

### Question B1-1

**Claude :**
Dans UMDD, plusieurs services détiennent des données : l'Annuaire a la liste des BP ouvertes, la Coopérative l'inventaire et la Banque les soldes des comptes.

Quelles sont les exigences particulières en matière de stockage de données dans un système distribué comme celui-ci ? Citez-en au moins deux et illustrez-les avec un service d'UMDD.

**Joe :**
> on doit pas perdre les données. on doit pas pouvoir lire les fichiers (sauf la banque et la coop)

**Claude (relance 1) :**
Concrètement, comment éviter de perdre les données de la Banque si son conteneur Docker est arrêté ou redémarré ?

Et que se passe-t-il si deux BP prélèvent en même temps la dernière spécialité disponible à la Coopérative ?

**Joe :**
> stocker en dehors du conteneur et même à double.
> en même temps c'est pas possible parce qu'il y a une seule coop. la deuxième fera erreur

### Question B1-2

**Claude :**
Vous devez déployer la Banque dockerisée de façon à ce que les soldes des comptes survivent à un redémarrage du conteneur.

Décrivez concrètement comment vous vous y prenez : quel système de stockage vous utilisez et comment vous le configurez avec Docker (commande, `Dockerfile` ou `docker-compose`, même approximatif).

**Joe :**
> jsp

### Question B1-3

**Claude :**
Pour chacun de ces trois services d'UMDD, quel système de gestion de données choisiriez-vous ? Justifiez chaque choix.

- l'Annuaire (liste des BP ouvertes)
- la Coopérative (inventaire des spécialités)
- la Banque (comptes et transferts)

Vous pouvez par exemple choisir entre aucun stockage persistant, un fichier JSON, une base relationnelle ou une base NoSQL.

**Joe :**
> annuaire: en mémoire parce que on envoie des message pour demander
> coop et banque: base de données parce que ça gère

**Claude (relance 1) :**
Pouvez-vous préciser « parce que ça gère » ? Quel type de base de données choisiriez-vous pour la Banque, et pourquoi ce type-là plutôt qu'un simple fichier JSON ?

**Joe :**
> mysql. ça peut avoir plusieurs clients en même temps

---

## Bande C1 – Expliquer, mettre en œuvre et évaluer les composants

### Question C1-1

**Claude :**
Dans UMDD, on trouve plusieurs types de composants : les BP, le broker MQTT, les microservices (Log, Annuaire, Chat, Coopérative, Banque) et les conteneurs Docker.

Expliquez le rôle de chacun de ces composants dans le système.

**Joe :**
> les bp et les services c'est des noeuds. le broker c'est pour communiquer entre tout le monde. docker c'est pour exécuter les noeuds (sauf les BP)

**Claude (relance 1) :**
Pourquoi exécute-t-on les services dans Docker mais pas les BP ? Qu'est-ce que Docker apporte concrètement à un service comme l'Annuaire ?

**Joe :**
> les BP elles sont sur les PC, docker sur le serveur.
> docker permet de lancer l'Annuaire facile

### Question C1-2

**Claude :**
Vous avez réalisé votre client `logger` en ligne de commande, avec les options `--help`, `--version`, `--broker` et `--source`.

Écrivez, même approximativement, le code C# qui traite ces arguments et démarre l'écoute du log.

Ensuite, expliquez comment vous avez testé que votre logger fonctionne correctement, d'abord en local puis avec les autres BP.

**Joe :**
```
if (args.count == 0) help()
if (args[0] == "--version") version()
if (args[0] == "--source") source = args[1]
_agent. connect (broker, "/logs/"+source
```
> j'ai lancé le conteneur sur mon PC et j'ai utilisé --broker

**Claude (relance 1) :**
Et pour le test avec les autres BP, comment avez-vous vérifié les points de la DoD ? Par exemple, afficher uniquement le log d'une BP précise pendant que les autres loggent, ou voir « ... died! » après avoir tué votre BP depuis le Task Manager.

**Joe :**
> j'ai organisé avec Julien

**Claude (relance 2) :**
Concrètement, qu'avez-vous fait avec Julien ? Quelles commandes avez-vous lancées et qu'avez-vous observé ?

**Joe :**
> on s'est connecté les deux, j'ai fait logger --source Julien

### Question C1-3

**Claude :**
Au moment de choisir l'Annuaire à dockeriser pour toute la classe, quatre implémentations développées par différents groupes étaient disponibles.

Sur quels critères évalueriez-vous ces quatre composants pour décider lequel intégrer au système ? Citez au moins trois critères et expliquez comment vous vérifieriez chacun d'eux.

**Joe :**
> le docker démarre facile. rien à faire et ça marche.
> il y a pas de bug
> Si c'est Julien, ça marche bien

**Claude (relance 1) :**
Comment vérifieriez-vous concrètement qu'« il n'y a pas de bug » ? Quels tests feriez-vous passer à chacun des quatre annuaires avant de choisir ?

**Joe :**
> comme on a fait: on se met à plusieurs et on regarde si on voit tout le monde. si y'en a un qui quitte, il disparait de mon bilboard

---

## Bande D1 – Expliquer et mettre en œuvre l'échange de données

### Question D1-1

**Claude :**
Dans UMDD, les BP et les services échangent des données via MQTT selon le modèle publish/subscribe.

Expliquez les principes de ce modèle d'échange de données : qui envoie, qui reçoit, comment le destinataire est déterminé. Comparez-le avec un modèle requête/réponse, comme un appel HTTP à une API REST.

**Joe :**
> quand je publish je sais pas qui va recevoir et je sais pas si quelqu'un a reçu. je sais pas si ma commande elle a marché. avec http, la réponse elle me dit ok ou pas

**Claude (relance 1) :**
Et comment le destinataire reçoit-il le message ? Quel est le rôle des topics et de l'abonnement (subscribe) ?

**Joe :**
> ceux qui veulent ils s'inscrivent pour un topic. quand un message arrive dessus, le broker il envoie à ceux qui sont inscrits

### Question D1-2

**Claude :**
Écrivez, même approximativement, le code C# de votre BP qui :

1. sérialise votre objet `Business` et l'enregistre dans un fichier ;
2. recharge ce fichier au démarrage pour reconstruire l'objet `Business`, à la place des valeurs codées en dur.

Expliquez ensuite comment vous avez vérifié que ça fonctionne.

**Joe :**
```
FILE.Writetext("me.json",JSONSerializer.Serialize(me))

Business me = JSONSerializer.Unserialize(File.Readtext("me.json"));
```
> avec le debugger

**Claude (relance 1) :**
Avec le débogueur, qu'avez-vous vérifié exactement ?

Et que devient l'image de l'enseigne, ou celle de la spécialité, lors de la sérialisation ?

**Joe :**
> point d'arrêt et j'ai inspecté l'objet avant et après
>
> du base64

### Question D1-3

**Claude :**
Vous l'avez dit vous-même : quand on publie sur MQTT, on ne sait pas si quelqu'un a reçu le message ni si la commande a fonctionné. Pourtant, pour un transfert bancaire, la BP doit savoir si l'opération a réussi.

Comment implémenteriez-vous l'échange entre une BP et la Banque pour répondre à cette exigence ? Vous pouvez rester sur MQTT ou choisir une autre technologie, mais justifiez votre choix. Décrivez les messages échangés (topics, contenu).

**Joe :**
> bp envoie transfert à la banque.
> la banque répond OK ou pas OK
> bp reçoit réponse. Si OK. bp demande solde à la banque. Sinon on fait rien
> si pas de réponse de la banque, on fait un log

**Claude (relance 1) :**
Sur quels topics la BP envoie-t-elle sa demande et la Banque sa réponse ?

Et si votre BP envoie deux transferts presque en même temps, comment sait-elle à quel transfert correspond chaque réponse « OK » ou « pas OK » ?

**Joe :**
> /money
>
> faut mettre un ID à la transaction

---

## Bande D2 – Expliquer et mettre en œuvre l'échange de données (protocoles)

### Question D2-1

**Claude :**
Citez au moins trois types de protocoles d'interface utilisables pour faire communiquer des composants dans un système distribué, par exemple MQTT. Pour chacun, indiquez ce qui le distingue des autres.

**Joe :**
> mqtt. c'est un bus de message
> http. on se connecte client / serveur
> https. pareil

**Claude (relance 1) :**
Quelle est la différence entre HTTP et HTTPS ? Connaissez-vous un autre type de protocole, par exemple pour une communication en temps réel dans les deux sens, ou pour des appels de procédure à distance ?

**Joe :**
> https c'est chiffré.
> jsp

### Question D2-2

**Claude :**
Pour le microservice Chat, il faut au minimum un topic qui sépare le trafic `chat` du reste du broker, un mécanisme d'envoi de message personnel et un mécanisme d'envoi de message global.

Proposez la structure des topics et le format des messages, puis écrivez, même approximativement, le code C# qui envoie un message personnel à Julien et celui qui reçoit les messages qui vous sont destinés.

**Joe :**
```
/chat/<destinataire>
_agent.Send("/chat/Julien","tcho")

(onMassageReceived)
if (message.topic.contains("Xavier")) // pour moi
```

**Claude (relance 1) :**
Comment envoyez-vous un message global avec cette structure de topics ?

Et comment Julien sait-il que le message « tcho » vient de vous ?

**Joe :**
```
_agent.Send("/chat/#", "tcho"
```
> dans l'envelope, il y a un senderid (_agent.nodeID)

**Claude (relance 2) :**
La DoD exige que personne d'autre n'ait vu l'échange entre vous et Julien. Avec la structure `/chat/<destinataire>`, est-ce garanti ? Pourquoi ?

**Joe :**
> non. tout le monde peut souscrire à /chat/# et tout voir

### Question D2-3

**Claude :**
Pour chacun des cas d'utilisation suivants, quel protocole d'interface jugez-vous le plus pertinent ? Justifiez chaque choix.

1. Le logging des BP (`Hello`, `Hey` toutes les 10 secondes, `Bye`, `died!`).
2. La consultation du solde d'un compte à la Banque.
3. Une application mobile publique qui affiche l'inventaire de la Coopérative à des clients de Dousville, hors du réseau de l'école.

**Joe :**
> jsp

**Claude (relance 1) :**
Prenons seulement le premier cas, le logging. MQTT est-il un bon choix, et pourquoi ?

**Joe :**
> oui. ça permet à n'importe qui de voir les logs n'importe où

**Claude (relance 2) :**
Et pour le troisième cas, l'application mobile publique qui affiche l'inventaire de la Coopérative à des clients hors du réseau de l'école : garderiez-vous MQTT ? Pourquoi ?

**Joe :**
> non. http ce serait mieux

---

## Bande E1 – Mise en œuvre de l'authentification et de l'autorisation

### Question E1-1

**Claude :**
Quelle est la différence entre l'authentification et l'autorisation ? Illustrez chacune avec un exemple tiré de la Banque d'UMDD.

**Joe :**
> authentification = t'es qui ? autorisation = t'as pas le droit de faire ça

**Claude (relance 1) :**
Pouvez-vous illustrer chacune avec un exemple concret tiré de la Banque ?

**Joe :**
> je fais un virement, je donne mon nom et celui à qui je verse. on peut signer le message avec ma clé privée.
> si je dis que je veux virer du compte de la coop sur le mien, la banque dit que j'ai pas le droit

### Question E1-2

**Claude :**
La thématique Sécurité exige de vérifier qu'une demande de transfert d'argent provient bien du détenteur du compte.

Décrivez la mise en œuvre de cette vérification par signature : ce que la BP fait avant d'envoyer la demande, ce que la Banque doit connaître au préalable, et ce qu'elle vérifie à la réception. Écrivez, même approximativement, le code C# côté BP et côté Banque.

Expliquez enfin comment vous testeriez que la vérification refuse bien une demande falsifiée.

**Joe :**
> chaque BP (PC élève) a la clé privée, la banque a les clés publiques de tout le monde.

```
Payment p = new Payment ("Joe","Bank",25);
p.sign(_privatekey);
_agent.Send("/money", JSONSerializer.Serialize(p));

// banque
Payment p = JSONSerializer.Deserialize(message);
pubkey = _keys.find(k => k.owner == p.source)
if (p.validate(pubkey)) // ok
```

**Claude (relance 1) :**
Que contiennent concrètement `sign()` et `validate()` ? Autrement dit, qu'est-ce qui est signé et comment la Banque vérifie-t-elle la signature ?

Et comment testeriez-vous qu'une demande falsifiée est bien refusée ?

**Joe :**
> du code de chiffrement RSA
> je bidouille la clé

**Claude (relance 2) :**
Plus précisément, sur quelles données la signature est-elle calculée ? Et qu'est-ce qui empêcherait quelqu'un d'intercepter votre demande de transfert signée sur le broker, puis de la renvoyer une deuxième fois telle quelle ?

**Joe :**
> jsp

### Question E1-3

**Claude :**
La thématique Sécurité exige aussi que les opérations bancaires, qui transitent par un broker ouvert, ne puissent être lues que par les personnes autorisées.

Quelle solution proposeriez-vous pour répondre à cette exigence ? Précisez quelles clés sont utilisées, par qui et pour quoi faire. Comparez-la avec une autre solution possible, par exemple sécuriser le broker lui-même.

**Joe :**
> chiffrer les messages sérialisés.
> on peu configurer MQTT avec TLS

**Claude (relance 1) :**
Pour chiffrer les messages sérialisés, quelle clé la BP utilise-t-elle pour chiffrer une demande destinée à la Banque, et quelle clé la Banque utilise-t-elle pour la déchiffrer ?

Et TLS sur le broker suffit-il à empêcher une autre BP de lire vos opérations bancaires ? Pourquoi ?

**Joe :**
> Clé publique de la banque pour chiffrer, clé privée pour déchiffrer
> oui

**Claude (relance 2) :**
Pourquoi TLS suffirait-il ? Une autre BP, connectée elle aussi au broker en TLS, ne pourrait-elle pas s'abonner au topic `/money` ?

**Joe :**
> oui, mais c'est pas chiffré pareil

---

## Bande F1 – Utilisation des outils de surveillance

### Question F1-1

**Claude :**
Qu'est-ce qu'un outil de surveillance (monitoring) d'un système distribué ? Expliquez à quoi il sert et ce qu'il surveille typiquement. Citez au moins un outil ou un mécanisme que vous connaissez, éventuellement parmi ceux utilisés dans UMDD.

**Joe :**
> une app qui pinge tous les services pour voir si ils sont là.
> powerwatch

**Claude (relance 1) :**
En dehors de la simple disponibilité, que peut-on surveiller d'autre ?

Et dans UMDD, quels mécanismes que vous avez déjà utilisés participent à la surveillance du système ?

**Joe :**
> le temps de réponse
> jsp

**Claude (relance 2) :**
Pensez au service Log et au message « ... died! » quand une BP est tuée depuis le Task Manager. Comment ce message est-il produit ?

**Joe :**
> LWT

### Question F1-2

**Claude :**
Vous êtes chargé de mettre en place l'entité de supervision de la thématique « Supervision » : elle doit détecter la panne d'un microservice (Annuaire, Chat, Coopérative, Banque).

Décrivez concrètement comment elle fonctionne : comment elle vérifie qu'un service est vivant, à quelle fréquence, et comment elle décide qu'il est en panne. Écrivez-en le code C#, même approximatif.

**Joe :**
> polling

**Claude (relance 1) :**
Pouvez-vous détailler ? Quel message envoie-t-elle, sur quel topic, à quelle fréquence, et à partir de quand considère-t-elle qu'un service est en panne ? Écrivez-en le code, même approximatif.

**Joe :**
```
while (true)
  _agent.Send("/log","ping")
  _agent.Send("/money","ping")
  _agent.Send("/directory","ping")
```

**Claude (relance 2) :**
Et comment la supervision reçoit-elle les réponses ? À partir de quand décide-t-elle qu'un service est en panne ?

**Joe :**
> onmessagereceived
> quand c'est > 5 secondes

### Question F1-3

**Claude :**
La supervision doit alerter l'équipe de maintenance quand un service tombe en panne.

Définissez les exigences de surveillance pour la Banque : que faut-il surveiller, avec quels seuils, et qui alerter, par quel moyen et à quel moment ? Expliquez ensuite comment vous configureriez votre outil de supervision en conséquence. Vous pouvez par exemple utiliser un fichier de configuration.

**Joe :**
> je fais un compte bidon. j'envoie de l'argent tout les 10 secondes, je regarde que ça augmente.
> si c'est faut ou que ça répond pas, email

**Claude (relance 1) :**
Comment configureriez-vous concrètement votre outil de supervision pour cela ? Qu'y aurait-il dans le fichier de configuration (services, seuils, destinataires) ? Et à qui l'email serait-il envoyé ?

**Joe :**
> dans le fichier:
> - timeout 5 secondes
> - alert = toto@eduvaud.ch

---

## Clôture

Fin de l'entretien à 14:45. Le rapport `Evaluation Joe.md` a été généré.
