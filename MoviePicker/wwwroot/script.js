async function loadLists() {
  try {
    const res = await fetch('/lists');
    const data = await res.json();
    document.getElementById('output').textContent = JSON.stringify(data, null, 2);
  } catch (err) {
    console.error('Error loading lists:', err);
  }
}
