use std::path::PathBuf;
use serde::{Deserialize, Serialize};
use crate::{Database, ShowResult};
use crate::api::episode::EpisodeResult;


#[derive(Debug, Deserialize, Serialize)]
pub struct Show {
    id: i64,
    name: String,
    path: PathBuf,
    episodes: Vec<Episode>,
}


#[derive(Debug, Deserialize, Serialize)]
pub struct Episode {
    id: i64,
    name: String,
    season: i32,
    episode: i32,
    is_special: bool,
}

impl Show {
    pub fn new() -> Self{
        Self{
            id: 0,
            name: "".to_string(),
            path: Default::default(),
            episodes: vec![]
        }
    }

    pub fn from(show: &ShowResult) -> Self{
        Self{
            id: show.id,
            episodes: Vec::new(),
            path: Default::default(),
            name: show.name.clone()
        }
    }

}

impl Episode{
    pub fn new() -> Self{
        Self{
            id: 0,
            name: "".to_string(),
            season: 0,
            episode: 0,
            is_special: false
        }
    }
    pub fn from(episode: &EpisodeResult) -> Self{
        Self{
            id: episode.id,
            name: episode.name.clone(),
            season: episode.season as i32,
            episode: episode.number.unwrap_or(0) as i32,
            is_special: episode.episode_type != "regular"
        }
    }
}

impl Database {
    pub fn add_show(&mut self, mut show: Show, path: Option<PathBuf>) {
        show.path = match path{
            Some(value) => value,
            None => {
                let mut dir = self.lib_dir.clone();
                dir.push(&show.name);
                dir
            }
        };
        self.shows.push(show)
    }
}