use serde::Deserialize;
use serde::Serialize;
use ureq::Error;

use super::endpoints::TvMazeEndpoint;

#[derive(Default, Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct EpisodeResult {
    pub id: i64,
    pub url: String,
    pub name: String,
    pub season: i32,
    pub number: Option<i32>,
    #[serde(rename = "type")]
    pub episode_type: String,
}

pub fn get_episodes(show_id: i64) -> Result<Vec<EpisodeResult>, Error> {
    let path = TvMazeEndpoint::Episodes(show_id).url();

    ureq::get(&path)
        .call()?
        .into_json::<Vec<EpisodeResult>>()
        .map_err(|e| e.into())
}
