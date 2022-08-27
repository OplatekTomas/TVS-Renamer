use crate::database::*;
use crate::ShowResult;
use serde::{Deserialize, Serialize};
use std::path::PathBuf;

#[derive(Debug, Deserialize, Serialize)]
pub struct Show {
    pub id: i64,
    pub name: String,
    pub path: PathBuf,
    pub episodes: Vec<Episode>,
    pub name_pattern: Vec<String>,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct Episode {
    pub id: i64,
    pub name: String,
    pub season: i32,
    pub episode: i32,
    pub is_special: bool,
}

impl From<ShowResult> for Show {
    fn from(show: ShowResult) -> Self {
        Self {
            id: show.id,
            episodes: Vec::new(),
            path: Default::default(),
            name: show.name.clone(),
            name_pattern: show
                .name
                .split_whitespace()
                .map(|x| x.to_lowercase())
                .collect(),
        }
    }
}

impl From<EpisodeResult> for Episode {
    fn from(episode: EpisodeResult) -> Self {
        Self {
            id: episode.id,
            name: episode.name.clone(),
            season: episode.season,
            episode: episode.number.unwrap_or(0),
            is_special: episode.episode_type != "regular",
        }
    }
}
