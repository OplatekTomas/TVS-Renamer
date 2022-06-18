pub mod database;

use std::path::PathBuf;
use std::fs;
use std::io::Write;
use database::*;
use serde::{Deserialize, Serialize};

#[derive(Deserialize, Serialize, Debug)]
pub struct Database {
    #[serde(skip_serializing, skip_deserializing)]
    path: PathBuf,
    shows: Vec<Show>,
    scan_dirs: Vec<PathBuf>,
}

impl Database {
    pub(crate) fn new() -> Self {
        Self {
            path: PathBuf::from("db.json"),
            shows: Vec::new(),
            scan_dirs: Vec::new(),
        }
    }
    pub(crate) fn load(&mut self) {
        if (!self.path.exists()) {
            self.save();
        }
        let reader = std::fs::File::open(self.path.clone());
        let file = match reader {
            Ok(file) => file,
            Err(e) => panic!("{}", e),
        };
        let db: Database = serde_json::from_reader(file).expect("Failed to read database");
        self.shows = db.shows;
        self.scan_dirs = db.scan_dirs;

    }
    fn save(&self) {
        let json = serde_json::to_string_pretty(&self).expect("Failed to serialize database");
        let mut file = fs::File::create(self.path.clone()).expect("Failed to create database file");
        file.write_all(json.as_bytes()).expect("Failed to write database file");
    }
}
