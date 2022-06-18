use std::path::PathBuf;
use serde::{Deserialize, Serialize};

#[derive(Debug,Deserialize,Serialize)]
pub struct Show{
    id: i32,
    name: String,
    path: PathBuf,
    episodes: Vec<Episode>
}
#[derive(Debug,Deserialize,Serialize)]
pub struct Episode{
    id: i32,
    name: String,
    season: i32,
    episode: i32,
    is_special: bool
}