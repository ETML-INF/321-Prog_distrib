# Fil rouge "UMDD (Un Monde De Douceurs)"

Dousville est la ville du Cyberspace qui compte le plus de Boulangeries-pâtisseries (BP).

Chacune d'entre elle produit une spécialité qu'elle vend fièrement.  
Mais les artisans de Dousville sont conscients que l'union fait la force et que la réputation de leur localité est importante pour leur prospérité individuelle.  
Plutôt que de se disputer le titre de meilleur boulangerie-pâtisserie, ils s'associent en coopérative et chacun vend également à ses clients les spécialités des autres boulangeries pâtisserie.

Tout ceci repose sur une architecture microservices:
- Log
- Registre du commerce
- Banque
- Dépôt de la coopérative
- Chat

## Thématique "Je suis sensé savoir..."

Objet / Classe / Static  
Listes d'objets, retrouver un objet (Contains)  
Constructeur (avec surcharge)  
Propriétés  

## Thématique (Dé)Sérialisation

Chacun crée un commerce: nom, enseigne (image), spécialité (nom + image + prix)  
Mise à disposition d'une base de code : une solution Windows Forms qui contient:
- Un modèle `Business` pour le commerce
- Un modèle `Specialty` pour la spécialité
- Un formulaire `Store` qui est la vitrine de la boulangerie
- Un formulaire `Billboard` qui affiche une `List<Business>`
- La "plomberie" Mqtt:
  - un agent qu'on instancie avec l'adresse IP du broker et le topic auquel ont souscrit
  - une méthode qui permet d'envoyer des messages sur un topic
  - une méthode appelée de manière asynchrone à la réception de message, cette méthode reçoit le topic complet en paramètres
 
### Activité
Afficher sa vitrine à partir de l'objet hardcodé  
Serialiser sa BP, la recharger à partir du du fichier et se débarrasser des valeurs hardcodées
Récupérer les BP des autres, afficher plusieurs BP dans le Billboard  

DoD:
- Chacun affiche sa vitrine et au moins deux autres BP

## Thématique Microservice: Logging

Chaque BP décide, quels sont les évènements qu'elle veut inscrire dans son journal (log).  
Ces évènements sont inscrits dans un service comment à tous : 
Broker `mqtt.blue.section-inf.ch`, topic `/logs/\<BPname>`

### Activité

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

## Thématique Microservice: Annuaire

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

## Thématique Microservice: Chat

Ce micro Service est mis à disposition de toutes les BP pour des échanges de messages informels entre elles.  
Le groupe qui le développe est responsable de fournir les informations nécessaires à l'utilisation du Service à l'ensemble de la classe.  
Au minimum:
- un topic permettant de séparer le trafic de type `chat` du reste sur le broker
- un mécanisme d'envoi de message personnel
- un mécanisme d'envoi de message global

DoD:
- j'envoie un message global, tout le monde le reçoit
- j'envoie un message personnel à un camarade, il le reçoit, me répond, je vois sa réponse, personne d'autre n'a vu l'échange
- le service est dockerisé
- les instructions d'utilisation du microservice sont mises à disposition de l'ensemble de la classe

## Thématique Microservice: Coopérative

Ce micro Service représente l'entrepôt dans lequel sont stockés les spécialités produites par chacune des BP.  
Avec lui, chaque BP peut:
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

La banque gère un compte courant pour chaque BP. Ce compte est initialisé avec un certain montant au démarrage de la banque.  
Avec lui, chaque BP peut:
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