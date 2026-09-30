## Thématique "Je suis sensé savoir..."

Objet / Classe / Static  
Listes d'objets, retrouver un objet (Contains)  
Constructeur (avec surcharge)  
Propriétés  

## Thématique (Dé)Sérialisation

Chacun crée un personnage: nom, argent, équipement, avatar  
Base de code : Contrôle "Persona Card", Mqtt et basta (Sans logger, crypto project, idempotence ...)  
Afficher sa carte à partir de l'objet (modèle->vue)  
Serialiser sa carte, la recharger à partir du du fichier  
Récupérer les cartes des autres, afficher plusieurs cartes  

## Thématique Microservice: Logging

Un seul broker blue, chacun logge dans `/logs/\<name>/#`
Un client log est mis à disposition
Chaque personne logge "Hello ..." au démarrage, "Coucou" toutes les 10 secondes et "Bye ..." à l'arrêt
Utiliser LWT pour les morts violentes

## Thématique Microservice: Annuaire

topic: `/directory`
Au démarrage, il envoie `whoisthere`
Les utilisateurs qui sont présents répondent avec `iamhere`
Les utilisateurs qui quitte s'annoncent avec `iamout`
Utiliser LWT pour les morts violentes
Un client directory est mis à disposition

Echanger des messages  
Echanger les cartes sérialisées par message plutôt que par fichier  
Structurer la comm:  
- microservices par topic name  
- se mettre d'accord sur un modèle commun -> protocole  
S’assurer du bon filtrage: no local + dest id  
Microservice: chat  
Microservice: Annuaire  
Utiliser LWT pour maintenir la liste  
Faire plusieurs petites app   
Microservice: Banque
  - la banque crée des comptes. les comptes sont distribués manuellement aux élèves
  - appels:
    - Solde
    - Dépôt
    - Retrait
Microservice: Arsenal. Il contient N instances de chaque classe: casque, plastron, épaulière, gants, pantalons, bottes, arme. Tout est "distribué" aléatoirement, ce qui fait que quelqu'un pourrait avoir trois casques, mais ni gants ni bottes.
    - Inventaire
    - Dépôt
    - Retrait
Microservice: Authentification
  - On lui donne un message signé, il répond OK ou pas
Microservice: vente de musique 
Projet: player standalone avec achat legit ET pirate P2P