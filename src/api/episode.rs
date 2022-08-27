use crate::TVMaze;
use serde::Deserialize;
use serde::Serialize;
use ureq::Error;

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

impl TVMaze {
    pub fn get_episodes(show_id: i64) -> Result<Vec<EpisodeResult>, Error> {
        let query = format!(
            "https://api.tvmaze.com/shows/{}/episodes?specials=1",
            show_id
        );

        ureq::get(&query)
            .call()?
            .into_json::<Vec<EpisodeResult>>()
            .map_err(|e| e.into())
    }
}
