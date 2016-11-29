using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Client.Models
{
    /// <summary>
    /// Strzalka oznaczajaca sortowanie, dodawana do kolumny z listview.
    /// </summary>
    public class SortAdorner : Adorner
    {
        private static Geometry ascGeometry = Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");
        private static Geometry descGeometry = Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="element">Element, do ktorego zostanie dodana strzalka.</param>
        /// <param name="dir">Kierunek strzalki</param>
        public SortAdorner(UIElement element, ListSortDirection dir)
            : base(element)
        {
            Direction = dir;
        }
        
        /// <summary>
        /// Obsluga rysowania
        /// </summary>
        /// <param name="drawingContext">Kontekst rysowania</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            var transform = new TranslateTransform(AdornedElement.RenderSize.Width - 15,
                (AdornedElement.RenderSize.Height - 5) / 2);
            drawingContext.PushTransform(transform);

            var geo = (Direction == ListSortDirection.Descending) ? descGeometry : ascGeometry;
            drawingContext.DrawGeometry(Brushes.White, null, geo);
            drawingContext.Pop();
        }
    }
}