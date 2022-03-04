const express = require("express");//il modulo express restituisce una funzione
const bodyParser = require("body-parser");
const fs = require('fs');
const app = express();

app.use(bodyParser.urlencoded({extended: true}));
app.use(bodyParser.json());

const fileUtenti = "utenti.json";
const filePalle = "palle.json";

let utenti;
let palle;

fs.readFile(fileUtenti, 'utf8', function(err, data) {
    if (err) throw err;
    utenti = data;
    utenti = JSON.parse(utenti);
});

fs.readFile(filePalle, 'utf8', function(err, data) {
    if (err) throw err;
    palle = data;
    palle = JSON.parse(palle);
});

//Server principale
app.listen(3000, () => console.log("Server JS in funzione!"));

app.get("/", (req, res) => { //Sto dicendo che se vado all'indirizzo base "/", viene eseguita la funzione 
    res.send("Server web in funzione!");
});

//Accesso ai dati totali
app.get("/data", (req, res) => {
    res.send(new Date().getDate().toString());
});

app.get("/palle", (req, res) => {
    res.send(palle);
});

app.get("/utenti", (req, res) => {
    res.send(utenti);
});

//Accesso ai dati specifici
app.get("/palle/:idPalla", (req, res) => {
    let idPalla = req.params.idPalla;
    let infoPalla = palle.voci.find(e => e.idPalla == idPalla);

    if (infoPalla) {
        res.send(
            "ID Palla: " + JSON.stringify(infoPalla.idPalla) + "<br>" +
            "Presa: " + JSON.stringify(infoPalla.presa) + "<br>" +
            "Posizione: " + JSON.stringify(infoPalla.posizioneX) + "," + JSON.stringify(infoPalla.posizioneY) + "<br>"
        );
    }
});

app.get("/utenti/:idUtente", (req, res) => {
    let idUtente = req.params.idUtente;
    let infoUtente = utenti.voci.find(e => e.idUtente == idUtente);

    if (infoUtente) {
        res.send(
            "Nome utente: " + JSON.stringify(infoUtente.nomeUtente) + "<br>" +
            "ID Utente: " + JSON.stringify(infoUtente.idUtente) + "<br>" +
            "Punteggio: " + JSON.stringify(infoUtente.punteggio) + "<br>" +
            "Ultima posizione: " + JSON.stringify(infoUtente.ultimaPosizioneX) + "," + JSON.stringify(infoUtente.ultimaPosizioneY) + "<br>"
        );
    }
});

//Impostazione dei dati specifici
app.post("/set-palla/:idPalla", (req, res) => {
    let idPalla = req.params.idPalla;
    let infoPalla = palle.voci.find(e => e.idPalla == idPalla);

    infoPalla["presa"] = true;

    fs.writeFile(filePalle, JSON.stringify(palle, null, 4), function(err) {
        if(err) { console.log(err); }
    }); 
});

app.post("/set-punti/:idUtente&:punteggio", (req, res) => {
    let idUtente = req.params.idUtente;
    let punteggio = req.params.punteggio;
    let infoUtente = utenti.voci.find(e => e.idUtente == idUtente);

    infoUtente["punteggio"] = parseInt(punteggio);

    fs.writeFile(fileUtenti, JSON.stringify(utenti, null, 4), function(err) {
        if(err) { console.log(err); }
    }); 
});

app.post("/set-utente/:idUtente&:posX&:posY", (req, res) => {
    let idUtente = req.params.idUtente;
    let posX = req.params.posX;
    let posY = req.params.posY;
    let infoUtente = utenti.voci.find(e => e.idUtente == idUtente);

    infoUtente["ultimaPosizioneX"] = parseInt(posX);
    infoUtente["ultimaPosizioneY"] = parseInt(posY);

    fs.writeFile(fileUtenti, JSON.stringify(utenti, null, 4), function(err) {
        if(err) { console.log(err); }
    }); 
});

app.post("/reset-palla/:idPalla", (req, res) => {
    let idPalla = req.params.idPalla;
    let infoPalla = palle.voci.find(e => e.idPalla == idPalla);

    infoPalla["presa"] = false;

    fs.writeFile(filePalle, JSON.stringify(palle, null, 4), function(err) {
        if(err) { console.log(err); }
    }); 
});