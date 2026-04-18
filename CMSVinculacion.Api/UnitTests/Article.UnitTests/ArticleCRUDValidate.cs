using Xunit;
using Moq;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Application.Services;
using CMSVinculacion.Application.DTOs.articles;
using CMSVinculacion.Domain.Entities.Contenido;

namespace Article.UnitTests
{
    public class ArticleCRUDValidate
    {
        //[Fact]
        //public void ArticleCreate_ReturnArticleInfo()
        //{

        //}

        private readonly Mock<IArticleRepository> _mockRepo;
        private readonly ArticleService _service;

        // El constructor en xUnit se ejecuta antes de cada prueba, 
        // dándonos un entorno limpio (Setup).
        public ArticleCRUDValidate()
        {
            _mockRepo = new Mock<IArticleRepository>();
            // Inyectamos el repositorio falso al servicio real
            _service = new ArticleService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetPublishedByIdAsync_ReturnsDto_WhenStatusIsPublished()
        {
            // 1. ARRANGE
            int articleId = 1;
            var fakeArticle = new Articles
            {
                ArticleId = articleId,
                Title = "Test",
                Status = new ArticleStatus { StatusName = "Published" }
            };

            // Le decimos al mock: "Cuando alguien llame a GetByIdAsync con el ID 1, devuelve fakeArticle"
            _mockRepo.Setup(repo => repo.GetByIdAsync(articleId, false))
                     .ReturnsAsync(fakeArticle);

            // 2. ACT
            var result = await _service.GetPublishedByIdAsync(articleId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
            Assert.Equal("Published", result.StatusName);
        }

        [Fact]
        public async Task GetPublishedByIdAsync_ReturnsNull_WhenStatusIsDraft()
        {
            // 1. ARRANGE
            int articleId = 2;
            var fakeArticle = new Articles
            {
                ArticleId = articleId,
                Title = "Borrador",
                Status = new ArticleStatus { StatusName = "Draft" } // Estado incorrecto
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(articleId, false))
                     .ReturnsAsync(fakeArticle);

            // 2. ACT
            var result = await _service.GetPublishedByIdAsync(articleId);

            // 3. ASSERT
            // El servicio debería devolver null porque no está publicado
            Assert.Null(result);
        }
        //prueba de create
        [Fact]

        public async Task CreateAsync_SanitizesHtml_AndCallsRepository()
        {
            // 1. ARRANGE
            var authorId = 99;
            var dto = new ArticleCreateDto
            {
                Title = "Nuevo Articulo",
                // Enviamos un script malicioso para ver si el HtmlSanitizer de tu servicio hace su trabajo
                ContentHtml = "<p>Hola</p><script>alert('xss')</script>",
                CategoryIds = new List<int> { 1, 2 }
            };

            // Le decimos al mock que simplemente devuelva el mismo artículo que recibe cuando llamen a CreateAsync
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Articles>()))
                     .ReturnsAsync((Articles a) => a);

            // 2. ACT
            var result = await _service.CreateAsync(dto, authorId);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("nuevo-articulo", result.Slug); // Verificamos tu generador de slugs
            Assert.Equal("<p>Hola</p>", result.ContentHtml); // Verificamos que el script fue eliminado

            // Verificamos que el repositorio realmente fue llamado exactamente 1 vez
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Articles>()), Times.Once);
        }
        // Prueba de update
        [Fact]
        public async Task UpdateAsync_ReturnsUpdatedDto_WhenArticleExists()
        {
            // 1. ARRANGE
            int articleId = 1;
            string updatedBy = "admin@test.com";

            var existingArticle = new Articles { ArticleId = articleId, Title = "Viejo Titulo" };
            var dto = new ArticleUpdateDto
            {
                Title = "Título Actualizado",
                ContentHtml = "<p>Contenido</p>",
                CategoryIds = new List<int> { 3, 4 }
            };

            // Simulamos que el repositorio encuentra el artículo al buscarlo
            _mockRepo.Setup(repo => repo.GetByIdAsync(articleId, false))
                     .ReturnsAsync(existingArticle);
            // 2. ACT
            var result = await _service.UpdateAsync(articleId, dto, updatedBy);

            // 3. ASSERT
            Assert.NotNull(result);
            Assert.Equal("Título Actualizado", result.Title); // Verificamos que se aplicó el cambio

            // Verificamos que los métodos del repositorio se hayan llamado correctamente
            _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Articles>()), Times.Once);
            _mockRepo.Verify(repo => repo.UpdateCategoriesAsync(articleId, dto.CategoryIds), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenArticleDoesNotExist()
        {
            // 1. ARRANGE
            int invalidId = 999;
            var dto = new ArticleUpdateDto { Title = "Test", ContentHtml = "<p>Test</p>" };

            // Simulamos que la base de datos no encuentra el ID
            _mockRepo.Setup(repo => repo.GetByIdAsync(invalidId, false))
                     .ReturnsAsync((Articles?)null);

            // 2. ACT
            var result = await _service.UpdateAsync(invalidId, dto, "admin");

            // 3. ASSERT
            Assert.Null(result); // Debe devolver null
            // Verificamos estrictamente que NO se haya intentado actualizar nada
            _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Articles>()), Times.Never);
        }

        //pruebas de delete
        [Fact]
        public async Task DeleteAsync_ReturnsTrue_AndCallsSoftDelete_WhenExists()
        {
            // 1. ARRANGE
            int articleId = 1;
            var existingArticle = new Articles { ArticleId = articleId };

            _mockRepo.Setup(repo => repo.GetByIdAsync(articleId, false))
                     .ReturnsAsync(existingArticle);

            // 2. ACT
            var result = await _service.DeleteAsync(articleId, "admin@test.com");

            // 3. ASSERT
            Assert.True(result); // Debe confirmar la eliminación
            // Verificamos que se llamó al método de borrado lógico con los datos correctos
            _mockRepo.Verify(repo => repo.SoftDeleteAsync(articleId, "admin@test.com"), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenDoesNotExist()
        {
            // 1. ARRANGE
            int invalidId = 999;
            _mockRepo.Setup(repo => repo.GetByIdAsync(invalidId, false))
                     .ReturnsAsync((Articles?)null);

            // 2. ACT
            var result = await _service.DeleteAsync(invalidId, "admin");

            // 3. ASSERT
            Assert.False(result); // Debe indicar que falló
            _mockRepo.Verify(repo => repo.SoftDeleteAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        //prueba del update status
        [Fact]
        public async Task UpdateStatusAsync_ReturnsTrue_WhenExists()
        {
            // 1. ARRANGE
            int articleId = 1;
            var existingArticle = new Articles { ArticleId = articleId };
            var dto = new ArticleStatusUpdateDto { StatusId = 2 }; // 2 = Published

            _mockRepo.Setup(repo => repo.GetByIdAsync(articleId, false))
                     .ReturnsAsync(existingArticle);

            // 2. ACT
            var result = await _service.UpdateStatusAsync(articleId, dto, "admin@test.com");

            // 3. ASSERT
            Assert.True(result);
            _mockRepo.Verify(repo => repo.UpdateStatusAsync(articleId, 2, "admin@test.com"), Times.Once);
        }
    }
}
