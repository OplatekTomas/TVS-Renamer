use ureq::Error;
use serde::Deserialize;
use serde::Serialize;

use super::endpoints::TvMazeEndpoint;

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct SearchResult {
    pub score: f64,
    pub show: ShowResult,
}

#[derive(Default, Debug, Clone, PartialEq, Eq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ShowResult {
    pub id: i64,
    pub url: String,
    pub name: String,
    pub premiered: Option<String>,
}

pub fn find_show_by_name_risky(name: &str) -> Result<ShowResult, Error> {
    let url = TvMazeEndpoint::SingleShow(name).url();
    let result: ShowResult = ureq::get(&url).call()?.into_json()?;

    Ok(result)

}

pub fn find_shows_by_name(name: &str) -> Result<Vec<ShowResult>, Error> {
    let url = TvMazeEndpoint::SearchShows(name).url();
    let result: Vec<SearchResult> = ureq::get(&url).call()?.into_json()?;
    let shows = result.into_iter().map(|item| item.show).collect();

    Ok(shows)
}
