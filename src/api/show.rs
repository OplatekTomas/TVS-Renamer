use serde::Deserialize;
use serde::Serialize;
use crate::TVMaze;
use reqwest::Error;
use ansi_term::Colour::{Green, Red};
use ansi_term::{Style};
use crate::helper::*;


#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct SearchResult {
    pub score: f64,
    pub show: ShowResult,
}

#[derive(Default, Debug, Clone, PartialEq, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ShowResult {
    pub id: i64,
    pub url: String,
    pub name: String,
    pub premiered: String
}


impl TVMaze {
    pub fn search(name: &String, risky: bool) -> Result<ShowResult, Error> {
        if risky{ //Returns the most probable result.
            let query = format!("https://api.tvmaze.com/singlesearch/shows?q={}", name);
            return reqwest::blocking::get(query)?.json();
        }
        //Queries all the shows and lets user select the correct one.
        let query = format!("https://api.tvmaze.com/search/shows?q={}", name);
        let mut result: Vec<SearchResult> = reqwest::blocking::get(query)?.json()?;
        println!("{}", Green.bold().paint("The API found the following shows:"));
        for (index, item) in result.iter().enumerate() {
            let bold = format!(
                "[{}]: {}",
                Style::new().bold().paint(index.to_string()),
                Style::new().bold().paint(&item.show.name)
            );
            println!("{} ({}), {}", bold, item.show.premiered, item.show.url);
        }
        let text = format!("Please select a show (0-{}): ", result.len() - 1);
        let mut index: usize;
        loop {
            index = read_number(Some(format!("{}", Green.bold().paint(&text)))) as usize;
            if index < result.len() {
                break;
            }
            println!("{}", Red.paint("value not in range"));
        }
        return Ok(result.remove(index).show);
    }
}
