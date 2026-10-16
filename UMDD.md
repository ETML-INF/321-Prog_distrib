# Fil rouge "UMDD (Un Monde De Douceurs)"

Nous sommes un groupe d'artisans Boulanger-Pâtissier qui a décidé de bâtir Dousville, la ville de nos rêves, celle des meilleures Boulangeries-Pâtisseries (BP) de tout le cyberspace.  
Nous disposons d'un terrain sur lequel bâtir notre ville:

(vue du terrain vide)

Un entrepreneur a déjà passé par là, il a mis à notre disposition un réseau de communication.  
Il vante le résultat de son travail de la manière suivante :

- Le réseau est fiable
- Il n'y a aucune latence sur le réseau
- Il offre une banque passante infinie
- Il est sûr
- Sa topologie est si bonne qu'il n'y aura jamais besoin de changer quoique ce soit
- Il n'a besoin que d'un seul administrateur (moi)
- Le coût des communications est nul
- Il est parfaitement homogène
- À tout endroit du réseau, chacun connaît l'heure avec précision

Un expert (M. Chad Gepeto) nous conseille de nous méfier de l'entrepreneur. Il nous recommande de nous organiser en coopérative et de construire ensemble une ville qui fonctionne sur ce réseau, mais sans croire aveuglement à ses affirmations. Il nous conseille de faire fonctionner notre ville sur une architecture microservices.

Il nous donne une liste de services qu'il estime nécessaire pour notre ville:

- TimeService
- LogService
- TradeService (Registre du commerce)
- MoneyService (Banque)
- StoreService (Dépôt de la coopérative)

Le résultat que nous visons ressemble à ceci:

(vue du terrain full)

# La route vers Dousville

## Thématique "Je suis sensé savoir..."

Avant de se lancer dans l'aventure, assurons-nous que nos bases sont solides.

- Objet / Classe / Static
- Listes d'objets, retrouver un objet (Contains)
- Constructeur (avec surcharge)
- Propriétés

### Activités

(TBD)

## Thématique (Dé)Sérialisation

Pour bien travailler ensemble, il va y avoir un certain nombre de classes qui sont partagées par tout le monde. Cela nous permettra d'échanger des objets entre deux programmes distants.

Pour cela, nous utiliserons la sérialisation.

### Activité

Chacun crée un commerce: nom, enseigne (image), spécialité (nom + image + prix)Mise à disposition d'une base de code : une solution Windows Forms qui contient:

- Un modèle `Business` pour le commerce
- Un modèle `Specialty` pour la spécialité
- Un formulaire `Store` qui est la vitrine de la boulangerie
- Un formulaire `Billboard` qui affiche une `List<Business>`
- La "plomberie" Mqtt:
  - un agent qu'on instancie avec l'adresse IP du broker et le topic auquel ont souscrit
  - une méthode qui permet d'envoyer des messages sur un topic
  - une méthode appelée de manière asynchrone à la réception de message, cette méthode reçoit le topic complet en paramètres

Afficher sa vitrine à partir de l'objet hardcodé
Serialiser sa BP, la recharger à partir du du fichier et se débarrasser des valeurs hardcodées
Récupérer les BP des autres, afficher plusieurs BP dans le Billboard

DoD:

- Chacun affiche sa vitrine et au moins deux autres BP

## Thématique protocole

Maintenant que nous sommes capables de sérialiser et désérialiser des objets, nous allons pouvoir nous les transmettre d'un nœud à l'autre.  
Le réseau que nous avons à notre disposition est certes fonctionnel, mais n'est pas très pratique pour s'envoyer et recevoir des objets sérialisés.

Nous allons maintenant choisir un protocole de niveau intermédiaire qui nous permettra de transporter nos objets d'un point à un autre.

### Activité

Sélection du protocole en commun orienter/limiter les choix vers MQTT / REST / RPC

## Thématique Microservice: Logging

Nous allons mettre en place un premier microservice qui ne fonctionne qu'en écriture: le `LogService`, qui reçoit tout message jugé utile par n'importe quel BP.

### Activité

Chaque BP décide, quels sont les évènements qu'elle veut inscrire dans son journal (log).
Ces évènements sont inscrits dans un service commun à tous :
Broker `mqtt.blue.section-inf.ch`, topic `/logs/\<BPname>`

Chacun réalisation de son client `logger`: une application en mode console:

```
# logger --help
usage : logger [--help] [--version] [--broker=<IP>] [--source=<name>]
  broker      nom ou adresse IP du broker MQTT. Par défaut: mqtt.blue.section-inf.ch
  source      topic à filtrer. Par défaut: `#`
```

DoD:

- J'ai mon client logger perso, `logger --version` affiche `Dousville logger ... , v1.0` avec mes initiales sur les `...`
- Ma BP logge "Hello from ..." au démarrage
- Ma BP logge "Hey from ..." toutes les 10 secondes
- Ma BP logge "Bye from ..." à l'arrêt
- Je peux montrer le log d'une BP précise uniquement, même quand les autres BP sont en train de logger des messages
- Je tue ma BP depuis le Task Manager, je peut voir "... died!" dans le log de ma BP
- Je peux montrer le log de toutes les BP simultanément

## Thématique Microservice : Time

À terme, chaque BP va publier ses horaires d'ouverture. Pour que le commerce fonctionne de manière harmonieuse à Dousville, il est donc nécessaire que tout le monde soit à la même heure.  
Nous allons maintenant mettre en place un microservice qui ne fonctionne qu'en lecture : `TimeService` qui diffuse en permanence leur actuelle à Dousville.

### Activité

Écrire notre TimeService, qui va lui-même se connecter sur un serveur NTP et qui diffuse l'heure actuelle (éventuellement corrigée) sur notre réseau

## Thématique Microservice: Annuaire

Nous allons maintenant mettre en place le microservice qui va nous permettre à chaque BP d'avoir connaissance de son environnement afin d'interagir avec lui.

Le service d'annuaire (SA) est responsable de maintenir à jour et publier une liste des BP ouvertes.
topic: `/directory`

### Comportement

Au démarrage, le SA envoie `whoisopen`, sans paramètres, adressage global. Comme il est sensé être toujours présent, il ne devrait y avoir personne sur réseau à son démarrage. Mais on rajoute quand même ce comportement pour le cas où il devrait être redémarré.
Les BP ouvertes répondent à `whoisopen` avec `iamopen`, avec leur modèle sérialisé, adressé à l'annuaire
Une BP qui ouvre s'annonce avec `iamopen`, avec son modèle sérialisé, adressé à l'annuaire
Une BP qui ferme s'annonce avec `iamclosed`, avec son modèle sérialisé, adressé à l'annuaire
À chaque ouverture/fermeture, l'annuaire envoie `open`, avec la liste des BP ouvertes, adressage global. Réflexion sur l'idempotence.
Utiliser LWT pour les morts violentes

### Activité

Par groupe de 3:

- créer un client directory (application console). Utiliser un topic privé pour le développement (`directory1` par exemple)
- adapter les BP de chacun

Un des quatre est momentanément retenu pour être notre annuaire.
On le dockerise pour qu'il soit disponible en permanence.
Les autres ne sont pas perdus : ils seront introduits comme backup un peu plus loin dans le cours (cluster).

DoD:

- on démarre une BP sur le poste A, elle s'affiche, elle est toute seule
- on démarre une deuxième BP sur le poste B:
  - elle s'affiche elle-même ainsi que l'autre
  - la deuxième BP apparaît sur le poste A
- on démarre une BP sur chacun des postes restant:
  - elle s'affiche elle-même ainsi que toutes les autres BP déjà démarrées
  - elle apparaît sur les postes de toutes les autres BP
- on arrête une BP gracieusement, elle disparaît de tous les postes
- on arrête une BP brutalement (task manager), elle disparaît de tous les postes

(trois microservices réalisés en groupe et en parallèle)

## Thématique P2P: Chat

Ce microservice est mis à disposition de toutes les BP pour des échanges de messages informels entre elles.Le groupe qui le développe est responsable de fournir les informations nécessaires à l'utilisation du Service à l'ensemble de la classe.Au minimum:

- un topic permettant de séparer le trafic de type `chat` du reste sur le broker
- un mécanisme d'envoi de message personnel
- un mécanisme d'envoi de message global

DoD:

- j'envoie un message global, tout le monde le reçoit
- j'envoie un message personnel à un camarade, il le reçoit, me répond, je vois sa réponse, personne d'autre n'a vu l'échange
- le service est dockerisé
- les instructions d'utilisation du microservice sont mises à disposition de l'ensemble de la classe

## Thématique Microservice: Coopérative

Ce microservice représente l'entrepôt dans lequel sont stockés les spécialités produites par chacune des BP.Avec lui, chaque BP peut:

- livrer des spécialités produites sous forme d'une liste d'objets `Specialty`
- demander l'inventaire complet actuel
- prélever une ou plusieurs spécialités

DoD:

- j'envoie 10 instance de ma spécialité à la coopérative
- j'obtiens l'inventaire de la coopérative
- je retrouve mes 10 spécialités dans l'inventaire
- je prélève une spécialité d'une autre BP
- le service est dockerisé
- les instructions d'utilisation du microservice sont mises à disposition de l'ensemble de la classe

## Thématique Microservice: Banque

La banque gère un compte courant pour chaque BP. Ce compte est initialisé avec un certain montant au démarrage de la banque.Avec lui, chaque BP peut:

- consulter son solde
- effectuer un transfert vers un autre compte
- recevoir une notification de versement

DoD:

- ma BP affiche l'état de mon compte
- la BP de mon collègue affiche l'état de son compte
- j'envoie un ordre de transfert de mon compte vers son compte pour un montant inférieur à mon solde: mon compte diminue et celui de mon collègue augmente
- le service est dockerisé
- les instructions d'utilisation du microservice sont mises à disposition de l'ensemble de la classe

## Thématique Transaction: Fair Trade

La coopérative dispose elle aussi d'un compte courant auprès de la banque.
À chaque fois qu'une BP dépose une production, la coopérative crédite le compte de la BP.
Dans le cas où le compte courant de la coopérative est insuffisamment garni pour payer l'artisan, celui-ci doit récupérer ses produits puisqu'ils n'ont pas été payés.

## Thématique Cluster

Le service d'annuaire est stateless. Il est donc particulièrement indiqué pour être déployé en cluster, afin d'en assurer la disponibilité et/ou distribuer la charge.

### Activité

Déployer les quatre services d'annuaire développés précédemment dans des conteneurs séparés.
Adapter chacune des applications de telle manière qu'à tout moment une et une seule des quatre instances de service ne répond aux clients.

## Thématique Sécurité

Jusqu'ici, nous avons tout fait à la confiance. N'importe qui peut déposer une spécialité qui n'est pas la sienne à la coopérative. N'importe qui peut consulter le compte courant de n'importe qui. Pire : n'importe qui peut faire un transfert d'argent d'un autre compte vers le sien.

On se respecte et on se fait confiance les uns les autres à Dousville, mais quand même ...

On va mettre en place des mécanismes cryptographiques qui vont nous permettre:

- de vérifier que la spécialité déposée à la coopérative provient bien de la BP qui est la seule à savoir la faire
- de vérifier que la demande de solde de compte provient bien du détenteur du compte
- de vérifier qu'une demande de transfert d'argent provient bien du détenteur du compte
- d'assurer que les opérations bancaires - qui transitent par un broker ouvert - ne peuvent être lues que par les personnes autorisées.

## Thématique Supervision

Pour que chaque BP puisse mener son activité quotidienne sereinement, on va mettre en place une entité dédiée à la surveillance de la disponibilité de tous les microservices. Ce système aura pour mission de détecter les pannes et d'alerter l'équipe de maintenance le cas échéant.
