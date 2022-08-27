use crate::{Database, Episode, Show};
use log::{error, info};
use std::ffi::OsStr;
use std::io::ErrorKind;
use std::path::{Path, PathBuf};
use walkdir::WalkDir;

pub struct Renamer<'a> {
    database: &'a Database,
}

impl<'a> Renamer<'a> {
    pub fn new(database: &'a Database) -> Self {
        Self { database }
    }

    pub fn rename_all_shows(&mut self) {
        let shows = &self.database.shows;
        for show in shows {
            self.rename_show(show.id);
        }
    }

    pub fn rename_show(&mut self, show_id: i64) {
        if self.database.scan_dirs.is_empty() {
            // TODO move
            eprintln!("No directories to scan from");
            return;
        }

        let show = self.database.get_show(show_id);
        if show.is_none() {
            error!("Show with id {} not found", show_id);
            return;
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
                    match std::fs::create_dir_all(&new_path) {
                        Ok(_) => info!("Created folder {:?}", new_path),
                        Err(e) if e.kind() != ErrorKind::AlreadyExists => {
                            error!("Error creating folder {:?}: {}", new_path, e)
                        }
                        _ => {}
                    }
                    new_path.push(new_name);
                    match std::fs::rename(&file, &new_path) {
                        Ok(_) => info!("Renamed {:?} to {:?}", file, new_path),
                        Err(e) => error!("Error renaming {:?}: {}", file, e),
                    }
                }
                None => continue,
            }
        }
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
        let extensions = ["mkv", "mp4", "avi", "mov", "srt"];
        let &db = &self.database;
        let mut all_files = Vec::new();
        let empty_ext = OsStr::new("");
        for directory in &db.scan_dirs {
            //Yeee this can be simplified with better query syntax
            for entry in WalkDir::new(directory).into_iter().filter_map(|e| e.ok()) {
                if !entry.file_type().is_file() {
                    continue;
                }
                let path = entry.into_path();
                let ext = path.extension().unwrap_or(empty_ext).to_str().unwrap();
                if extensions.iter().any(|&e| e == ext) {
                    all_files.push(path);
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
}
