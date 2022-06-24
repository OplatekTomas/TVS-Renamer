use std::fs;
use std::fs::{File, OpenOptions};
use std::io::{Error, ErrorKind, Write};
use std::path::PathBuf;
use ansi_term::Color::Red;
use chrono::Local;

pub struct Logger {
    pub log_file: String,
    file: Option<File>,
}

pub enum LogLevel {
    Info,
    Error,
}

impl Logger {
    pub fn new(log_file: String) -> Self {
        Self {
            log_file,
            file: None,
        }
    }

    pub fn open_log_file(&mut self) -> Result<(), Error> {
        let path = PathBuf::from(&self.log_file);
        let dir = path.parent();
        if dir.is_none() {
            return Err(Error::new(
                ErrorKind::Other,
                format!("Invalid log file path: {}", self.log_file),
            ));
        }
        fs::create_dir_all(dir.unwrap())?;

        let file = OpenOptions::new()
            .write(true)
            .append(true)
            .create_new(true)
            .open(&self.log_file);
        self.file = match file {
            Ok(file) => Some(file),
            Err(e) => return Err(e)
        };
        Ok(())
    }

    pub fn log(&mut self, message: String, level: LogLevel) {
        let time = Local::now().format("[%Y-%m-%dT%H:%M:%S]");
        let (console_text, file_text) = match level {
            LogLevel::Info => (message.clone(), format!("[{}] {}", time, message)),
            LogLevel::Error => (format!("{}", Red.bold().paint(&message)), format!("[{}] {}", time, message)),
        };
        match self.file {
            Some(ref mut file) => {
                let _ = file.write_all(&file_text.as_bytes());
            }
            None => {}
        };
        println!("{}", console_text);
    }
}