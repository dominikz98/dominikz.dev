namespace dominikz.api.Models.Structs;

public readonly struct NoteCollection
{
    public NoteData[] Notes { get; }

    public NoteCollection(List<NoteData> notes)
    {
        Notes = notes.ToArray();
    }

    public NoteData this[int index]
    {
        get => Notes[index];
        set => Notes[index] = value;
    }

    public NoteCollection(string value)
    {
        var parts = value.Split('$');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid note collection!");

        var idxString = parts[0];
        var notesWidthIdx = idxString.Split(':')
            .Select(x =>
            {
                var keys = x.Split('#');
                return new
                {
                    Id = int.Parse(keys[1]),
                    NoteAsString = keys[0]
                };
            })
            .ToDictionary(x => x.Id, x => x.NoteAsString);

        var positionsString = parts[1];
        Notes = positionsString.Split(':')
            .Select(x =>
            {
                var keys = x.Split('#');
                var id = int.Parse(keys[0]);
                var position = int.Parse(keys[1]);
                return new NoteData(notesWidthIdx[id], position);
            })
            .ToArray();
    }

    public override string ToString()
    {
        var notes = Notes.Distinct().ToList();
        var notesWithIdx = new Dictionary<string, int>();
        for (int i = 0; i < notes.Count; i++)
            notesWithIdx.Add(notes[i].ToString(), i);

        var idxString = string.Join(':', notesWithIdx.Select(x => $"{x.Key}#{x.Value}"));

        var positions = new List<string>();
        foreach (var note in Notes)
        {
            var idx = notesWithIdx[note.ToString()];
            positions.Add($"{idx}#{note.Position}");
        }

        var positionsString = string.Join(':', positions);
        return $"{idxString}${positionsString}";
    }
}
