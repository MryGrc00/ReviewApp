﻿using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IGenreService
    {
        List<GenreViewModel> GetGenres();
        GenreViewModel PaginatedGenres(int page, int pageSize);
        BookViewModel ViewGenreInBooks(string GenreName, int page, int pageSize);
        GenreViewModel GetGenre(int id);
        GenreViewModel GetGenreName(string genreName);
        void AddGenre(GenreViewModel model, string name);
        bool CheckGenreName(string GenreName);
        void UpdateGenre(GenreViewModel model, string name);
        void DeleteGenre(GenreViewModel model);
    }
}
