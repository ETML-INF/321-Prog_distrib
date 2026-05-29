Consolidation OO: objet / classe
Consolidation OO: liste d'objets, retrouver un objet désérialisé
Consolidation OO: Constructeur (avec surcharge)
Consolidation OO: Propriétés
Chacun crée un personnage au lieu de maison: nom, niveau, pièces, avatar
Base de code : Contrôle "Persona Card", Mqtt et basta. Sans logger, pas de crypto project, pas d'idempotence ...
Afficher sa carte à partir de l'objet (modèle->vue)
Serialiser sa carte, la recharger à partir du du fichier
Récupérer les cartes des autres, afficher plusieurs cartes
Microservice: logging
Un seul broker blue, isolation par topic privé 
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
Microservice: banque
    - la banque crée des comptes. les comptes sont distribués manuellement aux élèves
    - appels basiques:
      - Solde de compte
      - Transfert à un compte: retourne un reçu
    - appels sérieux:
      - Solde de compte: réponse si la demande est signée par le propriétaire du compte
      - Transfert à un compte: vérification de la signature, retourne un reçu signé de la banque
Microservice: Inventaire. Il contient N instances de chaque classe: casque, plastron, épaulière, gants, pantalons, bottes, arme. Tout est "distribué" aléatoirement, ce qui fait que quelqu'un pourrait avoir trois casques, mais ni gants ni bottes.
    - appels basiques:
      - Donne-moi la liste de mes objets
      - Donne-moi un objet (je connais son id)
      - Donne cet objet à untel
    - appels sérieux:
      - Je ne donne l'objet qu'après validaion du propriétaire
      - Je ne transmets l'objet qu'après validation du propriétaire
Microservice: vente de musique 
Projet: player standalone avec achat legit ET pirate P2P