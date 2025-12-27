# CineScore - spletna aplikacija
 
Člana ekipe:

63240036 **Tilen Butara**  
63230009 **Luka Ažman**

### Splošen Opis:
Spletna aplikacija CineScore je namenjena pregledu ocen in ocenjevanje filmov, ki so deljeni po letu izdaje, žanru in oceni, kar uporabnikom omogoča lažje iskanje po željenem atributu. Uporabniki imajo možnost registracije in prijave v račun, kar služi kot shramba vseh ocen in komentarjev, ki jih je uporabnik objavil, poleg tega bo račun imel tudi funkcije kot so "Favorites", "Watchlist", ...

#### Spletna Stran:
Spletna stran se bo delila na dva dela: uporabniški, ki bo dostopen vsem in bo vseboval vse zgoraj navedene funkcije in upraviteljski, ki bo dostopen le administratorjem in uporabljen za upravljanje z aplikacijo. Predvidena sestava spletne strani:

##### Uporabniški del:
Začetna stran kjer so navedeni filmi, Registracija/Prijava računa, Stran za upravljanje z računom, Stran za ocenjevanje filmov, Favorite list in Watchlist.
##### Upraviteljski del:
Stran za urejanje s filmi in Stran za urejanje z uporabniškimi računi.

#### API
REST API je na lokaciji `/api/v1/movies` in vrača podatke v JSON formatu. Za dostop je potreben API ključ, ki se pošlje v glavi `ApiKey`. Ključ nastavimo v `appsettings.json` (`ApiKey`), za produkcijo pa ga konfiguriramo preko spremenljivke okolja ali skrivnosti za Azure Web App.

#### Swagger dokumentacija
Swagger UI je na voljo na `/swagger` in prikazuje vse CRUD operacije za filme skupaj z možnostjo dodajanja API ključa v glavi zahteve.

##### Primeri klicev

Pridobivanje filmov:
```bash
curl -H "ApiKey: <tvoj_kljuc>" https://<tvoj-host>/api/v1/movies
```

Dodajanje novega filma:
```bash
curl -X POST -H "Content-Type: application/json" -H "ApiKey: <tvoj_kljuc>" \
  -d '{ "title": "Inception", "year": 2010, "genre": "Sci-Fi", "description": "Heist v sanjah" }' \
  https://<localhost:####>/api/v1/movies
```

