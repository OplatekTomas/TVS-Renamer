use crate::{Database, Episode, Show};
use log::info;
use std::error::Error;
use std::fmt::Display;
use std::io::ErrorKind;
use std::path::{Path, PathBuf};
use walkdir::WalkDir;

const EXTENSIONS: [&str; 5] = ["mkv", "mp4", "avi", "mov", "srt"];

pub struct Renamer<'a> {
    database: &'a Database,
}

impl<'a> Renamer<'a> {
    pub fn new(database: &'a Database) -> Self {
        Self { database }
    }

    pub fn rename_all_shows(&mut self) -> Result<(), RenamerError> {
        let shows = &self.database.shows;
        for show in shows {
            self.rename_show(show.id)?;
        }

        Ok(())
    }

    pub fn rename_show(&mut self, show_id: i64) -> Result<(), RenamerError> {
        if self.database.scan_dirs.is_empty() {
            return Err(RenamerError::NoScanDirs);
        }

        let show = self.database.get_show(show_id);
        if show.is_none() {
            return Err(RenamerError::IdNotFound(show_id));
        }
        let show = show.unwrap();
        //This can be cached and not queried again in case of multiple shows being renamed
        //OS probably does cache this for us
        let files = self.read_scan_dirs();

        for file in files {
            let episode = self.match_episode(show, &file);
            match episode {
                Some(episode) => {
                    let extension = file.extension().unwrap().to_str().unwrap();
                    let new_name = self.get_new_name(show, episode, extension);

                    let mut new_path = show.path.clone();
                    new_path.push(format!("Season {:02}", episode.season));

                    Self::create_sesaon_folder(&new_path)?;

                    new_path.push(new_name);
                    match std::fs::rename(&file, &new_path) {
                        Ok(_) => info!("Renamed {:?} to {:?}", file, new_path),
                        Err(e) => {
                            return Err(RenamerError::Io(format!("Error renaming {:?}", file), e));
                        }
                    }
                }
                None => continue,
            }
        }

        Ok(())
    }

    fn match_episode<'b>(&self, show: &'b Show, file: &Path) -> Option<&'b Episode> {
        let file_name = file.file_name().unwrap().to_str().unwrap().to_lowercase();
        for episode in &show.episodes {
            let mut parts = show.name_pattern.clone();
            parts.push(format!("e{:0>2}", episode.episode));
            parts.push(format!("s{:0>2}", episode.season));
            let is_episode = parts.iter().all(|pattern| file_name.contains(pattern));
            if is_episode {
                return Some(episode);
            }
        }
        None
    }

    fn read_scan_dirs(&mut self) -> Vec<PathBuf> {
        let &db = &self.database;
        let mut all_files = Vec::new();
        for directory in &db.scan_dirs {
            //Yeee this can be simplified with better query syntax
            for entry in WalkDir::new(directory).into_iter().filter_map(|e| e.ok()) {
                if !entry.file_type().is_file() {
                    continue;
                }

                if let Some(extension) = entry.path().extension() {
                    if extension.is_empty() {
                        continue;
                    }

                    if let Some(extension) = extension.to_str() {
                        if EXTENSIONS.contains(&extension) {
                            all_files.push(entry.into_path());
                        }
                    }
                }
            }
        }

        all_files
    }

    fn get_new_name(&self, show: &Show, episode: &Episode, extension: &str) -> String {
        format!(
            "{} - S{:0>2}E{:0>2} - {}.{}",
            &show.name, episode.season, episode.episode, episode.name, extension
        )
    }

    fn create_sesaon_folder(path: &Path) -> Result<(), RenamerError> {
        match std::fs::create_dir_all(&path) {
            Ok(_) => {
                info!("Created folder {:?}", path);
                Ok(())
            }
            Err(e) if e.kind() != ErrorKind::AlreadyExists => Err(RenamerError::Io(
                format!("Error creating folder {:?}", path),
                e,
            )),
            _ => Ok(()),
        }
    }
}

#[derive(Debug)]
pub enum RenamerError {
    Io(String, std::io::Error),
    NoScanDirs,
    IdNotFound(i64),
}

impl Display for RenamerError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            Self::Io(msg, err) => f.write_fmt(format_args!("IO Error - {}: {}", msg, err)),
            Self::NoScanDirs => f.write_str("No directories to scan from"),
            Self::IdNotFound(id) => {
                f.write_fmt(format_args!("Show with id {} not found in db", id))
            }
        }
    }
}

impl From<std::io::Error> for RenamerError {
    fn from(e: std::io::Error) -> Self {
        Self::Io(String::new(), e)
    }
}

impl Error for RenamerError {
    fn source(&self) -> Option<&(dyn Error + 'static)> {
        match self {
            Self::Io(_, e) => Some(e),
            _ => None,
        }
    }
}
