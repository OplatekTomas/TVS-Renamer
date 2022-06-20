pub mod database;

use std::path::PathBuf;
use std::fs;
use std::io::Write;
use database::*;
use serde::{Deserialize, Serialize};

#[derive(Deserialize, Serialize, Debug)]
pub struct Database {
    #[serde(skip_serializing, skip_deserializing)]
    db_path: PathBuf,
    pub shows: Vec<Show>,
    pub scan_dirs: Vec<PathBuf>,
    pub initialized: bool,
    pub lib_dir: PathBuf
}

impl Database {
    pub(crate) fn new() -> Self {
        Self {
            db_path: PathBuf::from("db.json"),
            shows: Vec::new(),
            scan_dirs: Vec::new(),
            initialized: false,
            lib_dir: PathBuf::new()
        }
    }
    pub(crate) fn load(&mut self) -> bool {
        if !self.db_path.exists() {
            self.save();
            return false;
        }
        let reader = std::fs::File::open(self.db_path.clone());
        let file = match reader {
            Ok(file) => file,
            Err(e) => panic!("{}", e),
        };
        let db: Database = serde_json::from_reader(file).expect("Failed to read database");
        self.shows = db.shows;
        self.scan_dirs = db.scan_dirs;
        self.initialized = db.initialized;
        self.lib_dir = db.lib_dir;
        return self.initialized;
    }
    pub(crate) fn save(&self) {
        let json = serde_json::to_string_pretty(&self).expect("Failed to serialize database");
        let mut file = fs::File::create(self.db_path.clone()).expect("Failed to create database file");
        file.write_all(json.as_bytes()).expect("Failed to write database file");
    }
}
