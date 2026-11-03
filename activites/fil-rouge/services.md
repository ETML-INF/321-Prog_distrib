# Catalogue de services

Un [scénario](README.md) choisit, pour chaque catégorie dont il a besoin, un service parmi les options possibles (un mélange est permis) et le déclare dans son [tableau de services](modele/README.md#services-choisis). Le [fil rouge libre](libre/README.md) peut ajouter une catégorie, à condition de dire quelle [exigence](README.md#contrat-de-scenario) elle remplit.

## Catégories

| Catégorie | Rôle | Introduite par | Options possibles |
|---|---|---|---|
| <span id="cat-source-de-temps"></span>Source de temps | donner à tous la même heure | [Temps](../../thematiques/02-temps.md), [MQTT](../../thematiques/03-mqtt.md) | service de temps synchronisé sur NTP · « Mère Nature » qui diffuse heure et météo · horloge de la ville · horloge commune des usines · horloge de jeu accélérée |
| <span id="cat-annuaire"></span>Annuaire | dire qui est vivant et où le joindre | [Microservices](../../thematiques/04-microservices.md) | annuaire des commerces · registre du commerce · registre des maisons · catalogue des stations |
| <span id="cat-journal"></span>Journal | centraliser les traces | [Microservices](../../thematiques/04-microservices.md) | journal central · journal d'audit · boîte noire des pannes |
| <span id="cat-paiement"></span>Paiement | tenir les comptes et exécuter les transferts | [Idempotence](../../thematiques/05-idempotence.md) | banque · distributeur de cash · porte-monnaie par nœud · caisse commune |
| <span id="cat-echange"></span>Échange | une opération entre deux parties qui doit réussir en entier | [Transactions](../../thematiques/06-transactions.md) | coopérative · marché d'énergie · commande de matière première à plusieurs fournisseurs · troc entre voisins |
| <span id="cat-vitrine"></span>Vitrine / état public | publier l'état visible d'un nœud | [Sérialisation](../../thematiques/01-serialisation.md) | vitrine de commerce · état d'une maison · fiche d'usine · tableau d'affichage |
| <span id="cat-supervision"></span>Supervision | observer la santé et alerter | [Supervision](../../thematiques/10-supervision.md), [Énergie](../../thematiques/11-energie.md) | moniteur de battements · surveillant de maisons · tableau de bord · alertes par seuils |
| <span id="cat-identite"></span>Identité et confiance | prouver l'auteur, protéger les données | [Sécurité](../../thematiques/09-securite.md) | autorité de clés · registre de clés publiques · réseau de confiance |
| <span id="cat-passerelle"></span>Passerelle / adaptateur | faire parler deux protocoles | [Sérialisation](../../thematiques/01-serialisation.md), [Sécurité](../../thematiques/09-securite.md) | traducteur de protocole externe · pont HTTP ↔ MQTT · adaptateur d'un ancien format |
| <span id="cat-stockage"></span>Stockage d'état | survivre à un redémarrage | [Résilience](../../thematiques/07-resilience.md) | sauvegarde périodique · journal d'événements rejouable · petite base clé-valeur |
| <span id="cat-simulateur"></span>Simulateur d'événements | produire du monde et des pannes | [Temps](../../thematiques/02-temps.md), [Résilience](../../thematiques/07-resilience.md) | météo · flux de clients · générateur de pannes aléatoires |
| <span id="cat-coordinateur"></span>Coordinateur | décider quand plusieurs peuvent répondre | [Cluster et P2P](../../thematiques/08-cluster-p2p.md) | élection d'un chef · répartiteur de charge · verrou partagé |
| <span id="cat-defi"></span>Défi | mettre l'ensemble à l'épreuve | toutes | défi voté · tour de ville sous pannes · épreuve d'énergie |

## Exemples de services

Chaque exemple donne ce qu'un service de la catégorie fait **sur le bus**, avec la racine `{racine}` du scénario. Les noms de types et de topics sont **à adapter** : à écrire par le groupe dans le contrat avant de coder. Aucun topic ci-dessous (`directory`, `pay/…`, `trade/…`, `keys/…`, `leader/…`, `alerts`…) n'appartient au [noyau](../contrats/README.md#topics-du-noyau) : le groupe les fixe et les fait relire (voir [contrats écrits par la classe](../contrats/README.md#contrats-ecrits-par-la-classe)).

### Source de temps

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| un serveur NTP public | `{racine}/time` (type `time`, QoS 0) à intervalle régulier | interroger NTP, estimer sa propre erreur, diffuser l'heure ; reprendre sans salve de rattrapage | NTP injoignable (mode dégradé documenté), broker coupé |

### Annuaire

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| `{racine}/presence/+` (ouvert, fermé, mort) | `{racine}/directory` (liste retenue, type à définir) | ajouter un nœud qui s'annonce, retirer un nœud mort (LWT) en moins de 10 s, répondre à « qui est là ? » | un nœud mort dont le message retenu reste, redémarrage de l'annuaire |

### Paiement

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| `{racine}/pay/request` (débit, crédit, transfert ; `id` unique) | `{racine}/pay/result` (accepté, refusé, doublon) | exécuter une demande **une seule fois** même reçue deux fois, refuser « fonds insuffisants », exposer un solde | doublons, messages hors d'ordre, redémarrage (mémoire de déduplication bornée) |

### Échange

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| offre et acceptation entre deux nœuds | `{racine}/trade/<id>` (étapes : proposé, réservé, payé, livré, annulé) | enchaîner réservation et paiement, **compenser** en cas d'échec, ne jamais livrer sans paiement | panne entre deux étapes, acceptation reçue deux fois |

### Identité et confiance

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| demande d'enregistrement d'une clé publique | `{racine}/keys/<nœud>` (clé publique, retenue) | enregistrer, publier, révoquer une clé ; fournir de quoi vérifier une signature | clé révoquée encore en circulation, rejeu d'un ancien message signé |

### Supervision

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| `{racine}/log/+` (battements `hb …`, voir le [contrat](../contrats/README.md#battement-heartbeat)) | `{racine}/alerts` (silence, seuil dépassé) | signaler un nœud silencieux après `3 × N` secondes, ni avant ni bien après ; limiter son propre trafic | moniteur redémarré (état perdu), fausse alerte après une coupure du broker |

### Passerelle / adaptateur

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| messages d'un protocole externe | enveloppes du [contrat de base](../contrats/README.md) | traduire dans les deux sens, conserver l'`id` d'origine, rejeter ce qui ne se traduit pas | message externe invalide, l'un des deux côtés indisponible |

### Coordinateur

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| présences des instances d'un service | `{racine}/leader/<service>` (retenu) | désigner **une seule** instance active, la remplacer si elle meurt | deux instances qui se croient chef (partition), élection pendant une coupure |

### Vitrine / état public

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| l'état local d'un nœud (fiche, mesures, offre) | `{racine}/<vitrine>/<nœud>` (type métier, **retenu**) | sérialiser l'état vers un fichier et le recharger, publier, afficher celui des autres, ignorer une fiche invalide sans effacer les autres | arrivée tardive (doit voir l'existant), fiche trop lourde, JSON cassé |

### Journal

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| `{racine}/log/+` (type `log`) | un fichier ou une vue ordonnée ; éventuellement `{racine}/log-index` | écouter sans jamais publier, classer par nœud et niveau, signaler la mort annoncée (LWT) d'un nœud | rafale de messages, `ts` incohérents (ne pas s'en servir pour l'ordre), redémarrage |

### Stockage d'état

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| l'état d'un autre service, ou ses événements | une copie rechargeable (fichier, base) | écrire sans perdre l'état précédent si le processus meurt en cours d'écriture, recharger au démarrage, annoncer qu'il est prêt | crash pendant l'écriture, disque plein, état plus ancien que le broker |

### Simulateur d'événements

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| une graine et un rythme (réglables) | événements du monde sur `{racine}/<flux>` (météo, clients, pannes) | produire des événements **reproductibles** (même graine, même suite), pouvoir être accéléré ou arrêté, ne jamais être indispensable aux autres | arrêt brutal (le monde se fige, rien ne casse) |

### Défi

| Entrées | Sorties | Opérations attendues | Pannes à tolérer |
|---|---|---|---|
| le scénario voté, les services existants | une page « fallacies en jeu », un déroulé et un critère de réussite | décider du défi **avant** de coder, nommer les fallacies visées, définir ce qui compte comme réussi | tout ce que l'enseignant provoque avec le [labo](../infra/README.md) |

Voir aussi l'[exemple de répartition en équipe](modele/equipe.md).
