# UMDD · La vitrine

Avant de bâtir la ville, chacun ouvre sa boulangerie-pâtisserie (BP) et on se regarde les uns les autres.

|           |                                                                                          |
|-----------|------------------------------------------------------------------------------------------|
| Prérequis | [D01](../../drills/D01-serialisation.md) · [C1](../../chat/C1-hello.md)                  |
| Fallacy   | 3 (bande passante), 8 (réseau homogène)                                                  |
| Contrat   | [business](../../contrats/business.schema.json), topic `dousville/vitrine/<bp>` (retain) |

## Mission

1. Définissez votre BP : nom, enseigne (image), au moins deux spécialités (nom, image, prix).
2. **Affichez** votre vitrine à partir de valeurs écrites en dur. Interface libre : terminal, page web, ou le `Front` C# fourni.
3. **Sérialisez** votre BP vers un fichier JSON conforme au schéma, rechargez-la depuis le fichier et supprimez les valeurs en dur.
4. **Publiez** votre vitrine (message `business`) sur `dousville/vitrine/<bp>`.
5. **Abonnez-vous** à `dousville/vitrine/+` et affichez dans un *Billboard* toutes les BP reçues.

## DoD

- Je montre ma vitrine et celles d'au moins **deux autres BP**, dont **une écrite dans un autre langage que le mien**.
- Une BP lancée *après* les autres voit tout de suite les vitrines déjà publiées (sans que personne ne republie).
- Un message qui ne valide pas le schéma n'efface pas le Billboard.

## Indices

- Pourquoi `retain` ici ? (voir [D03](../../drills/D03-qos-retain.md)). Que se passe-t-il quand une BP ferme ? Vous le réglerez avec l'[annuaire](log-annuaire.md).
- Mesurez la taille d'une vitrine avec image. Redimensionnez l'image à ≤ 20 Ko avant de l'encoder (fallacy 3).
- Si vous utilisez le `Front` C# ([activites/umdd](../../umdd/README.md)) : il fournit un `Business`, une `Specialty`, un `Store` et un `Billboard`, mais **pas** votre contrat. À vous de l'adapter.
- Une image en base64 dans un JSON : c'est le plus simple, est-ce le plus sobre ? Notez un avantage et un inconvénient.

## Pannes à provoquer (labo)

- Ajoutez 2 s de latence (voir [infra](../../infra/README.md)) : votre Billboard reste-t-il réactif ?
- Publiez une vitrine hostile à la main (`price` négatif, JSON cassé) : que se passe-t-il ?
