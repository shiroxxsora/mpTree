using Microsoft.AspNetCore.Mvc;
using MpTree.DBControl;
using System; // For Exception
using System.Collections.Generic;
using System.Threading.Tasks; // For Task, though DAO is sync for now
using Microsoft.AspNetCore.Http; // For StatusCodes

namespace MpTree.Controllers
{
    /// <summary>
    /// API контроллер для управления музыкальными файлами.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly SongDao _songDao;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SongsController"/>.
        /// </summary>
        /// <param name="songDao">Объект доступа к данным для песен.</param>
        public SongsController(SongDao songDao)
        {
            _songDao = songDao;
        }

        /// <summary>
        /// Создает новую песню.
        /// </summary>
        /// <param name="song">Данные песни для создания. Путь и длительность обязательны и проверяются.</param>
        /// <response code="201">Возвращает новую песню и её местоположение.</response>
        /// <response code="400">Если данные песни null, недействительны (например, отсутствует путь/длительность или неверные значения согласно ограничениям модели) или если песню не удалось создать в базе данных.</response>
        /// <response code="500">Если произошла внутренняя ошибка сервера во время обработки.</response>
        [HttpPost]
        [ProducesResponseType(typeof(SongModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateSong([FromBody] SongModel song)
        {
            if (song == null)
            {
                return BadRequest("Данные песни отсутствуют.");
            }

            if (string.IsNullOrWhiteSpace(song.Path) || song.Duration <= 0) 
            {
                return BadRequest("Неверный путь или длительность. Путь должен быть корректным путем Windows, а длительность должна быть положительной.");
            }
            
            try
            {
                int result = _songDao.CreateSong(song);
                if (result > 0)
                {
                    return CreatedAtAction(nameof(GetSongByName), new { name = song.Name }, song);
                }
                return BadRequest("Не удалось создать песню. Пожалуйста, убедитесь, что все поля корректны, и попробуйте снова.");
            }
            catch (ArgumentException ex) 
            {
                return BadRequest($"Неверные данные песни: {ex.Message}");
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Произошла ошибка при создании песни: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает список всех песен.
        /// </summary>
        /// <response code="200">Возвращает список всех песен.</response>
        /// <response code="500">Если произошла внутренняя ошибка сервера.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<SongModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllSongs()
        {
            try
            {
                var songs = _songDao.GetAllSongs();
                return Ok(songs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Произошла ошибка при получении списка песен: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает конкретную песню по её названию.
        /// </summary>
        /// <param name="name">Название песни для получения.</param>
        /// <response code="200">Возвращает песню с указанным названием.</response>
        /// <response code="400">Если название песни null или пустое.</response>
        /// <response code="404">Если песня с указанным названием не найдена.</response>
        /// <response code="500">Если произошла внутренняя ошибка сервера.</response>
        [HttpGet("{name}")]
        [ProducesResponseType(typeof(SongModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSongByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Название не может быть пустым.");
            }
            try
            {
                var song = _songDao.GetSong(name);
                if (song == null)
                {
                    return NotFound($"Песня с названием '{name}' не найдена.");
                }
                return Ok(song);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Произошла ошибка при получении песни: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновляет существующую песню. Песня идентифицируется по 'name' в маршруте
        /// и обновляется с использованием данных из тела запроса.
        /// </summary>
        /// <param name="name">Оригинальное название песни для обновления. Используется для идентификации песни.</param>
        /// <param name="songToUpdate">Данные песни для обновления. Свойство `Name` внутри этого объекта будет использоваться как ключ для обновления в базе данных. Путь и длительность обязательны.</param>
        /// <response code="204">Если песня была успешно обновлена.</response>
        /// <response code="400">Если предоставленные данные песни или название null/пустые, или если проверка модели не прошла (например, неверный путь/длительность).</response>
        /// <response code="404">Если песня с указанным названием (из `songToUpdate.Name`) не найдена для обновления.</response>
        /// <response code="500">Если произошла внутренняя ошибка сервера во время обновления.</response>
        [HttpPut("{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateSong(string name, [FromBody] SongModel songToUpdate)
        {
            if (songToUpdate == null || string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Данные песни или параметр name не могут быть null/пустыми.");
            }

            if (string.IsNullOrWhiteSpace(songToUpdate.Path) || songToUpdate.Duration <= 0)
            {
                return BadRequest("Неверный путь или длительность в предоставленных данных песни. Путь должен быть корректным путем Windows, а длительность должна быть положительной.");
            }

            try
            {
                int result = _songDao.UpdateSong(songToUpdate);
                if (result > 0)
                {
                    return NoContent(); 
                }
                return NotFound($"Песня с названием '{songToUpdate.Name}' не найдена для обновления, или данные идентичны.");
            }
            catch (ArgumentException ex) 
            {
                return BadRequest($"Неверные данные песни: {ex.Message}");
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Произошла ошибка при обновлении песни: {ex.Message}");
            }
        }

        /// <summary>
        /// Удаляет песню по пути к файлу.
        /// </summary>
        /// <param name="path">URL-закодированный путь к файлу песни для удаления. Сервер декодирует этот путь.</param>
        /// <response code="204">Если песня была успешно удалена.</response>
        /// <response code="400">Если путь null или пустой.</response>
        /// <response code="404">Если песня с указанным путем не найдена для удаления.</response>
        /// <response code="500">Если произошла внутренняя ошибка сервера.</response>
        [HttpDelete("{path}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteSongByPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return BadRequest("Путь не может быть пустым.");
            }

            try
            {
                int result = _songDao.DeleteSong(path); 
                if (result > 0)
                {
                    return NoContent(); 
                }
                return NotFound($"Песня с путем '{path}' не найдена для удаления.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при удалении песни: {ex.Message}");
            }
        }
    }
}
