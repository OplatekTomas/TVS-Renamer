use reqwest::Error;
use crate::TVMaze;
use serde::Deserialize;
use serde::Serialize;


#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct EpisodeResult {
    pub id: i64,
    pub url: String,
    pub name: String,
    pub season: i64,
    pub number: Option<i64>,
    #[serde(rename = "type")]
    pub episode_type: String,
}


impl TVMaze {
    pub fn get_episodes(show_id: i64) -> Result<Vec<EpisodeResult>, Error> {
        let query = format!("https://api.tvmaze.com/shows/{}/episodes?specials=1", show_id);
        return reqwest::blocking::get(query)?.json();
    }

}