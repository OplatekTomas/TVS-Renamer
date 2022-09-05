const ROOT: &str = "https://api.tvmaze.com";

#[derive(Debug)]
pub enum TvMazeEndpoint<'a> {
    SearchShows(&'a str),
    SingleShow(&'a str),
    Episodes(i64),
}

impl<'a> TvMazeEndpoint<'a> {
    pub fn url(self) -> String {
        match self {
            TvMazeEndpoint::SearchShows(name) => format!("{}/search/shows?q={}", ROOT, name),
            TvMazeEndpoint::SingleShow(name) => format!("{}/singlesearch/shows?q={}", ROOT, name),
            TvMazeEndpoint::Episodes(show_id) => format!("{}/shows/{}/episodes?specials=1", ROOT, show_id),
        }
    }
}
