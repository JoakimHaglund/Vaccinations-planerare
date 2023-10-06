# Inlämmningsuppgift 4 - Vaccinationsplanerare
Beskrivning av programmet.

## ToDo lista:

### Klass - Validering (ValidateInput)
- [x] Skapa metod för att läsa heltal från användaren.
- [x] Skapa metod för att hämta indatafil.
  * Vid inläsning av sökväg till indatafil: be om ny inmatning om den angivna filen inte finns.
- [x] Skapa metod för att hämta utdatafil.
  * Vid inläsning av sökväg till utdatafil: be om ny inmatning om den angivna mappen inte finns.

### Klass - Filhantering (FileIo)
- [ ] Skapa metod för att spara csv fil.
- [ ] Skapa metod för att läsa csv fil.
  * Vid inläsning av CSV-fil: skriv ut ett felmeddelande för varje felaktig rad i CSV-filen och återgå till huvudmenyn istället för att skapa en prioritetsordning.
- [ ] Skapa metod för att konventera från klassobjekt till csv format.
- [ ] Skapa metod för att konventera från csv format till klassobjekt.
- [x] Skapa metod för att välja fil.
- [x] Skapa metod för att välja map.

### Klass - Person information (PersonalInformation) 
- [x] Skapa variabler.
  * Förnamn
  * Efternamn
  * Personnummer (förutsätt att personer är födda på 1900-talet)
  * Jobbar vår/omsorg
  * Riskgrupp
  * Resterande

### Huvudprogram
- [x] Skapa huvudmeny.
- [x] Visa information om filsökvägar, antal vaccindoser och vaccinering av minderåriga.
- [x] Skapa alternativ för att välja antal tillgängliga vaccindoser.
- [x] Skapa alternativ för att välja vaccinering av minderåriga.
- [ ] Skapa alternativ för att ändra indatafil.
- [ ] Skapa alternativ för att ändra utdatafil.
- [ ] Implementera prioritetsordning.

    **Regler för prioritetsordning:**

    > * Inom varje fas ska vaccineringarna tilldelas efter ålder: äldst först. Även månad och dag ska tas i med denna beräkning (inte bara år).
    > * Personer yngre än 18 år ska enbart vaccineras om användaren väljer detta som alternativ (se exemplet nedan). Om detta alternativ väljs ska dessa personer ingå i de fyra faserna enligt samma               regler som för alla andra. Om detta alternativ inte väljs ska ingen person yngre än 18 vaccineras oavsett omständigheterna, även om de exempelvis tillhör en riskgrupp.
    > * Personer som redan har genomgått en infektion ska vaccineras med enbart en dos. Övriga ska vaccineras med två doser.
    >  Om det enbart finns en dos kvar och nästa person i ordningen ska vaccineras med två doser så ska denna person inte tilldelas några doser. Kvarvarande personer ska inte heller tilldelas några              doser, även om någon av dem bara ska vaccineras med en dos (på grund av genomgången infektion). Med andra ord ska den sista dosen antingen användas till en fullständig vaccination av nästa person         i ordningen eller inte användas alls.
   > * Antalet tillgängliga vaccindoser ska inte ändras efter att en prioritetsordning har skapats. Denna inställning ska alltså förbli samma oavsett hur många prioritetsordningar som skapas, tills              användaren själv ändrar antalet tillgängliga vaccindoser med det aktuella menyvalet.
  * Sortering av prioritetsordning: Anställda inom vård och omsorg -> Ålder -> Riskgrupp -> Övriga
  *  **_Vid skapande av prioritetsordning: om utdatafilen redan existerar så ska användaren bekräfta om de vill skriva över den._**

## VG-Del.

- [ ] Lägg till menyval för Schemaläggning av vaccinationerna.
- [ ] Spara Schemat som en iCalandar-fil (.ics).
  *  [Länk till iCalandar-spec](https://datatracker.ietf.org/doc/html/rfc5545)


    
    
    
