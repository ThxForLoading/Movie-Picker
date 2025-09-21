import { useState, useEffect } from "react";

function App() {
  const [username, setUsername] = useState("");
  const [movieTitle, setMovieTitle] = useState("");
  const [movies, setMovies] = useState([]);
  const [allLists, setAllLists] = useState([]);

  useEffect(() => {
    fetch("http://localhost:5292/lists") // fetch all lists
      .then(res => res.json())
      .then(data => setAllLists(data))
      .catch(err => console.error(err));
  }, []);

  const addMovie = () => {
    if (!movieTitle || movies.length >= 25) return;
    setMovies([...movies, { rank: movies.length + 1, title: movieTitle, posterUrl: "" }]);
    setMovieTitle("");
  };

  const submitList = async () => {
    if (!username || movies.length === 0) return;

    const payload = {
      username,
      movies: movies.map((m, i) => ({
        rank: i + 1,
        title: m.title,
        posterUrl: m.posterUrl || ""
      }))
    };

    console.log("Submitting payload:", payload);

    try {
      const res = await fetch("http://localhost:5292/submit-list", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      });

      if (res.ok) {
        const newList = await res.json();
        setAllLists([...allLists, newList]);
        setMovies([]);
        setUsername(username);
        alert("List submitted!");
      }
    } catch (err) {
      console.error("Error submitting list:", err);
    }
  };

  return (
    <div style={{ padding: "20px", fontFamily: "Arial" }}>
      <h1>Movie Picker</h1>

      <div style={{ marginBottom: "10px" }}>
        <input
          placeholder="Your name"
          value={username}
          onChange={e => setUsername(e.target.value)}
        />
      </div>

      <div style={{ marginBottom: "10px" }}>
        <input
          placeholder="Movie title"
          value={movieTitle}
          onChange={e => setMovieTitle(e.target.value)}
        />
        <button onClick={addMovie} style={{ marginLeft: "10px" }}>Add Movie</button>
      </div>

      <h3>Current list ({movies.length}/25)</h3>
      <ul>
        {movies.map(m => (
          <li key={m.rank}>{m.rank}. {m.title}</li>
        ))}
      </ul>

      <button onClick={submitList} style={{ marginTop: "10px" }}>Submit List</button>

      <hr />

      <h2>All Lists</h2>
      {allLists.map(list => (
        <div key={list.id} style={{ border: "1px solid #ccc", marginBottom: "10px", padding: "5px" }}>
          <strong>{list.username}</strong>
          <ol>
            {(list.movies || []).map(m => (
              <li key={m.rank}>{m.title}</li>
            ))}
          </ol>
        </div>
      ))}
    </div>
  );
}

export default App;
