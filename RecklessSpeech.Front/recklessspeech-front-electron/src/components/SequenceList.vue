<script>
import axios from "axios";

export default {
  data() {
    return {
      filePickerDialog: false,
      jsonPickerDialog: false,
      words: [],
      checkedWords: [],
      selectedFile: null,
      selectedJson: null,
    };
  },
  methods: {
    async enrichInDutch() {
      const selectedWords = this.words.filter((word, index) => {
        return this.checkedWords[index];
      });
      for (const sequence of selectedWords) {
        let id = sequence.id;
        await axios.post(
          `https://localhost:47973/api/v1/sequences/Dictionary/dutch?id=${id}`
        );
      }
      let msg =
        selectedWords.length +
        " séquences ont été enrichies avec succès en néérlandais.";
      console.log(msg);
      new Notification(msg).show();
    },
    async enrichInEnglish() {
      const selectedWords = this.words.filter((word, index) => {
        return this.checkedWords[index];
      });
      for (const sequence of selectedWords) {
        let id = sequence.id;
        await axios.post(
          `https://localhost:47973/api/v1/sequences/Dictionary/english?id=${id}`
        );
      }

      let msg =
        selectedWords.length +
        " séquences ont été enrichies avec succès en anglais.";
      console.log(msg);
      new Notification(msg).show();
    },
    async sendToAnki() {
      const selectedWords = this.words.filter((word, index) => {
        return this.checkedWords[index];
      });
      for (const sequence of selectedWords) {
        let id = sequence.id;
        await axios.post(
          `https://localhost:47973/api/v1/sequences/send-to-anki?id=${id}`
        );
      }

      let msg =
        selectedWords.length +
        " séquences ont été envoyées vers Anki avec succès.";
      console.log(msg);
      new Notification(msg).show();
    },
    openFilePicker() {
      this.filePickerDialog = true;
    },
    openJsonPicker() {
      this.jsonPickerDialog = true;
    },
    onFileSelected(event) {
      this.selectedFile = event.target.files[0];
    },
    onJsonSelected(event) {
      this.selectedJson = event.target.files[0];
    },
    selectAll() {
      if (this.checkedWords.some((isChecked) => isChecked)) {
        // Au moins un élément est sélectionné, donc on les désélectionne tous
        this.checkedWords = [];
      } else {
        // Aucun élément n'est sélectionné, donc on les sélectionne tous
        this.checkedWords = this.words.map(() => true);
      }
    },
    async importCSV() {
      try {
        const formData = new FormData();
        formData.append("file", this.selectedFile);

        await axios
          .post("https://localhost:47973/api/v1/sequences", formData, {
            headers: {
              "Content-Type": "multipart/form-data",
            },
          })
          .then(
            axios
              .get("https://localhost:47973/api/v1/sequences")
              .then((response) => {
                this.words = response.data;
                console.log(
                  this.words.length + " words set into the variable 'words'."
                );
                new Notification(
                  this.words.length + " mots ont été importés."
                ).show();
              })
              .catch((error) => {
                console.error(error);
              })
          );
      } catch (error) {
        console.error(error);
        let msg =
          "Une erreur est survenue lors de l'importation du fichier CSV.";
        console.log(msg);
        new Notification(msg).show();
      }

      this.filePickerDialog = false;
    },
    async importJSON() {
      try {
        const formData = new FormData();
        formData.append("file", this.selectedJson);

        const response = await axios.post(
          "https://localhost:47973/api/v1/sequences/import-details",
          formData,
          {
            headers: {
              "Content-Type": "multipart/form-data",
            },
          }
        );
        console.log(
          "import-details http call ended with status: " + response.status
        );
        // Mettre à jour la liste des séquences si besoin
      } catch (error) {
        console.error(error);

        let msg =
          "Une erreur est survenue lors de l'importation du fichier Json.";
        console.log(msg);
        new Notification(msg).show();
      }

      this.jsonPickerDialog = false;
    },
  },
};
</script>
<template>
  <div>
    <div class="fieldset-container">
      <fieldset style="border: 2px solid #000; padding: 10px">
        <legend style="font-size: 20px">Sélectionner un fichier CSV</legend>
        <p>
          <input
            type="file"
            ref="fileInput"
            accept=".csv"
            @change="onFileSelected"
          />
          <button @click="importCSV">Importer</button>
        </p>
      </fieldset>
    </div>

    <div class="fieldset-container">
      <fieldset style="border: 2px solid #000; padding: 10px">
        <legend style="font-size: 20px">Sélectionner un fichier JSON</legend>
        <p>
          <input
            type="file"
            ref="fileInput"
            accept=".json"
            @change="onJsonSelected"
          />
          <button @click="importJSON">Importer</button>
        </p>
      </fieldset>
    </div>

    <div class="fieldset-container">
      <fieldset style="border: 2px solid #000; padding: 10px">
        <legend style="font-size: 20px">Enrichir</legend>
        <button class="clickable button-margin" @click="selectAll()">
          Selectionner tout
        </button>
        <button class="clickable button-margin" @click="enrichInEnglish()">
          Enrichir en anglais
        </button>
        <button class="clickable button-margin" @click="enrichInDutch()">
          Enrichir en néérlandais
        </button>
      </fieldset>
    </div>
    <div class="fieldset-container">
      <fieldset style="border: 2px solid #000; padding: 10px">
        <legend style="font-size: 20px">Envoyer</legend>
        <button class="clickable button-margin" @click="sendToAnki()">
          Envoyer vers Anki
        </button>
      </fieldset>
    </div>

    <div>
      <table class="table">
        <tbody>
          <tr v-for="(file, index) in words" :key="file.name">
            <td>
              <input
                type="checkbox"
                class="checkboxes"
                v-model="checkedWords[index]"
              />
              <span>{{ file.word }}</span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>


<style scoped>
.clickable {
  cursor: pointer;
}
.button-margin {
  margin: 5px;
}
.fieldset-container {
  margin: 5px;
}
.checkboxes {
  margin: 5px;
}
</style>