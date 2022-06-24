
use std::path::PathBuf;
use crate::database::*;
use serde::{Deserialize, Serialize};
use crate::ShowResult;


#[derive(Debug, Deserialize, Serialize)]
pub struct Show {
    pub id: i64,
    pub name: String,
    pub path: PathBuf,
    pub episodes: Vec<Episode>,
    pub name_pattern: Vec<String>
}


#[derive(Debug, Deserialize, Serialize)]
pub struct Episode {
    pub id: i64,
    pub name: String,
    pub season: i32,
    pub episode: i32,
    pub is_special: bool,
}

impl Show {
    pub fn new() -> Self{
        Self{
            id: 0,
            name: "".to_string(),
            path: Default::default(),
            episodes: vec![],
            name_pattern: vec![]
        }
    }

    pub fn from(show: &ShowResult) -> Self{
        Self{
            id: show.id,
            episodes: Vec::new(),
            path: Default::default(),
            name: show.name.clone(),
            name_pattern: show.name.split_whitespace().map(|x| x.to_lowercase()).collect()
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
            season: episode.season,
            episode: episode.number.unwrap_or(0),
            is_special: episode.episode_type != "regular"
        }
    }
}
